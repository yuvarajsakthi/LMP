import { createContext, useContext, useState, type ReactNode } from 'react';

interface LayoutContextType {
  collapsed: boolean;
  setCollapsed: (collapsed: boolean) => void;
}

const LayoutContext = createContext<LayoutContextType | undefined>(undefined);

export const LayoutProvider = ({ children }: { children: ReactNode }) => {
  const [collapsed, setCollapsed] = useState(false);

  return (
    <LayoutContext.Provider value={{ collapsed, setCollapsed }}>
      {children}
    </LayoutContext.Provider>
  );
};

export const useLayout = () => {
  const context = useContext(LayoutContext);
  if (!context) {
    throw new Error('useLayout must be used within a LayoutProvider');
  }
  return context;
};