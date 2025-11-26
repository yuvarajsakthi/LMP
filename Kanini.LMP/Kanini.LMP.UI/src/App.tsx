import { BrowserRouter } from "react-router-dom"
import { AuthProvider } from "./context"
import { ErrorBoundary } from "./components"
import { AppRoutes } from "./config"

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <ErrorBoundary>
          <AppRoutes />
        </ErrorBoundary>
      </AuthProvider>
    </BrowserRouter>
  )
}

export default App
