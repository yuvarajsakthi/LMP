# LMP Deployment Guide - Jenkins CI/CD to Azure VM

## Prerequisites Setup

### 1. Jenkins Configuration

#### Install Required Plugins:
- Docker Pipeline
- SSH Agent
- Credentials Binding
- Git

#### Add Credentials in Jenkins (Dashboard → Manage Jenkins → Credentials):

1. **dockerhub-credentials** (Username with password)
   - ID: `dockerhub-credentials`
   - Username: Your DockerHub username
   - Password: Your DockerHub password/token

2. **github-credentials** (Username with password or SSH key)
   - ID: `github-credentials`
   - Your GitHub credentials

3. **azure-vm-ssh** (SSH Username with private key)
   - ID: `azure-vm-ssh`
   - Username: `azureuser`
   - Private Key: Your VM SSH private key

4. **Secret Text Credentials** (Add each as separate Secret Text):
   - `db-connection-string`: Your Azure SQL connection string
   - `jwt-key`: Your JWT secret key
   - `smtp-host`: smtp.gmail.com
   - `smtp-port`: 587
   - `smtp-username`: Your email
   - `smtp-password`: Your email app password

### 2. Azure VM Setup (Ubuntu)

SSH into your VM and run:

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install Java (required for Jenkins)
sudo apt install -y openjdk-17-jdk
java -version

# Install Jenkins
curl -fsSL https://pkg.jenkins.io/debian-stable/jenkins.io-2023.key | sudo tee /usr/share/keyrings/jenkins-keyring.asc > /dev/null
echo deb [signed-by=/usr/share/keyrings/jenkins-keyring.asc] https://pkg.jenkins.io/debian-stable binary/ | sudo tee /etc/apt/sources.list.d/jenkins.list > /dev/null
sudo apt update
sudo apt install -y jenkins
sudo systemctl start jenkins
sudo systemctl enable jenkins
sudo systemctl status jenkins

# Get initial admin password
sudo cat /var/lib/jenkins/secrets/initialAdminPassword

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER
sudo usermod -aG docker jenkins

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Restart Jenkins to apply docker group
sudo systemctl restart jenkins

# Create deployment directory
mkdir -p /home/azureuser/lmp
cd /home/azureuser/lmp

# Copy docker-compose.yml to this directory (from your repo)
```

**Access Jenkins**: http://YOUR_VM_IP:8080
- Use the initial admin password from above
- Install suggested plugins
- Create admin user

### 3. Update Jenkinsfile Variables

Edit `Jenkinsfile` and update:
- `DOCKERHUB_USERNAME`: Your DockerHub username
- `GITHUB_REPO`: Your GitHub repository URL
- `VITE_API_BASE_URL`: http://YOUR_VM_PUBLIC_IP:5000/api
- SSH command: Replace `your-vm-ip` with your Azure VM public IP

### 4. Azure Network Security Group

Open ports in Azure NSG:
- Port 80 (HTTP - Frontend)
- Port 5000 (Backend API)
- Port 8080 (Jenkins UI)
- Port 22 (SSH - for Jenkins deployment)

### 5. Create Jenkins Pipeline

1. Jenkins Dashboard → New Item
2. Enter name: `LMP-Deployment`
3. Select: Pipeline
4. Pipeline Definition: Pipeline script from SCM
5. SCM: Git
6. Repository URL: Your GitHub repo
7. Credentials: Select github-credentials
8. Branch: */main
9. Script Path: Jenkinsfile
10. Save

## Deployment Flow

```
GitHub → Jenkins → Build Docker Images → Push to DockerHub → Deploy to Azure VM
```

## Manual Deployment (Alternative)

If you want to manually copy the Jenkinsfile content:

1. Jenkins Dashboard → New Item → Pipeline
2. Select "Pipeline script" (not SCM)
3. Copy entire Jenkinsfile content into the script box
4. Update variables directly in the script
5. Save and Build

## Environment Variables in Docker

**Backend** uses build args that become environment variables:
- Connection strings, JWT keys, SMTP, Razorpay, WhatsApp configs

**Frontend** uses build arg:
- `VITE_API_BASE_URL` - baked into the build

## Verify Deployment

After successful pipeline run:

```bash
# Check running containers
ssh azureuser@your-vm-ip
docker ps

# Check logs
docker logs lmp-backend
docker logs lmp-frontend

# Test endpoints
curl http://your-vm-ip:5000/api/health
curl http://your-vm-ip
```

## Troubleshooting

**Build fails**: Check Jenkins console output for errors
**Docker push fails**: Verify DockerHub credentials
**Deployment fails**: Check SSH connectivity and VM Docker installation
**App not accessible**: Verify Azure NSG rules and container logs

## Security Notes

- Never commit credentials to Git
- Use Jenkins credentials store for all secrets
- Rotate credentials regularly
- Use HTTPS in production (add SSL certificate to Nginx)
- Restrict SSH access to Jenkins server IP only
