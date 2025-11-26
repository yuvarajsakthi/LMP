import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { SnackbarProvider } from 'notistack'
import './index.css'
import App from './App.tsx'

const rootElement = document.getElementById('root');
if (!rootElement) {
  throw new Error('Root element not found');
}

try {
  createRoot(rootElement).render(
    <StrictMode>
      <SnackbarProvider maxSnack={3}>
        <App />
      </SnackbarProvider>
    </StrictMode>,
  );
} catch (error) {
  console.error('Failed to render React app:', error);
}
