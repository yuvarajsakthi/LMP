import React from 'react';
import { ErrorBoundary as ReactErrorBoundary } from 'react-error-boundary';
import { Result, Button } from 'antd';
import { motion } from 'framer-motion';
import ErrorBoundaryCss from './ErrorBoundary.module.css';

function ErrorFallback({ error, resetErrorBoundary }: { error: Error; resetErrorBoundary: () => void }) {
  return (
    <motion.div 
      className={ErrorBoundaryCss.errorBoundary}
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.3 }}
    >
      <Result
        status="error"
        title="Something went wrong"
        subTitle={error.message || "An unexpected error occurred. Please try again."}
        extra={[
          <Button type="primary" key="retry" onClick={resetErrorBoundary}>
            Try Again
          </Button>,
          <Button key="home" onClick={() => window.location.href = '/'}>
            Go Home
          </Button>
        ]}
      />
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