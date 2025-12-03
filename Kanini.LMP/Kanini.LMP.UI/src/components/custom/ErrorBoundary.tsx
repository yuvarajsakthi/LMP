import React from 'react';
import { ErrorBoundary as ReactErrorBoundary } from 'react-error-boundary';
import { Result, Button } from 'antd';
import { motion } from 'framer-motion';
import ErrorBoundaryCss from './ErrorBoundary.module.css';

function ErrorFallback({ error, resetErrorBoundary }: { error: Error; resetErrorBoundary: () => void }) {
  const handleGoHome = () => {
    window.location.href = '/customer-dashboard';
  };

  return (
    <motion.div 
      className={ErrorBoundaryCss.errorBoundary}
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.3 }}
    >
      <Result
        status="error"
        title="Oops! Something went wrong"
        subTitle="We're sorry for the inconvenience. Please try reloading or go back to the dashboard."
        extra={[
          <Button type="primary" key="retry" onClick={resetErrorBoundary}>
            Try Again
          </Button>,
          <Button key="home" onClick={handleGoHome}>
            Go to Dashboard
          </Button>
        ]}
      />
      {process.env.NODE_ENV === 'development' && (
        <details style={{ marginTop: 20, padding: 10, background: '#f5f5f5', textAlign: 'left' }}>
          <summary>Error Details (Development Only)</summary>
          <pre style={{ whiteSpace: 'pre-wrap', fontSize: '12px' }}>{error.stack}</pre>
        </details>
      )}
    </motion.div>
  );
}

function ErrorBoundary({ children }: { children: React.ReactNode }) {
  return (
    <ReactErrorBoundary
      FallbackComponent={ErrorFallback}
      onError={(error, errorInfo) => {
        console.error('Error caught by boundary:', error, errorInfo);
      }}
      onReset={() => {
        window.location.reload();
      }}
    >
      {children}
    </ReactErrorBoundary>
  );
}

export default ErrorBoundary;