import { type ReactNode } from 'react';

export interface GuardProps {
  children: ReactNode;
}

export type InputChangeEvent = React.ChangeEvent<HTMLInputElement>;