import { BrowserRouter } from "react-router-dom"
import { Provider } from 'react-redux'
import { AuthProvider, LayoutProvider } from "./context"
import { ErrorBoundary } from "./components"
import { AppRoutes } from "./config"
import { store } from "./store"

function App() {
  return (
    <Provider store={store}>
      <BrowserRouter>
        <AuthProvider>
          <LayoutProvider>
            <ErrorBoundary>
              <AppRoutes />
            </ErrorBoundary>
          </LayoutProvider>
        </AuthProvider>
      </BrowserRouter>
    </Provider>
  )
}

export default App