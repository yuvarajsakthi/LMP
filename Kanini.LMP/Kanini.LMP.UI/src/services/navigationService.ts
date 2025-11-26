import { type NavigateFunction } from 'react-router-dom';

let navigate: NavigateFunction | null = null;

export const navigationService = {
  setNavigate: (navigateFunction: NavigateFunction) => {
    navigate = navigateFunction;
  },

  navigateTo: (path: string, replace = false) => {
    if (navigate) {
      navigate(path, { replace });
    }
  }
};