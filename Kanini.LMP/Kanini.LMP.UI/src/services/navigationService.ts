import { type NavigateFunction } from 'react-router-dom';

let navigate: NavigateFunction | null = null;

export const navigationService = {
  setNavigate: (navigateFunction: NavigateFunction) => {
    navigate = navigateFunction;
  },

  navigateTo: (path: string, replace = false) => {
    if (navigate) {
      try {
        navigate(path, { replace });
      } catch (error) {
        console.error('Navigation error:', error);
      }
    } else {
      console.warn('Navigation function not set');
    }
  },

  goBack: () => {
    if (navigate) {
      navigate(-1);
    }
  },

  isNavigateReady: () => {
    return navigate !== null;
  }
};