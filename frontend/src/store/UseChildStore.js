import { create } from 'zustand';

export const UseChildStore = create((set) => ({
  currentChild: null,

  setCurrentChild: (child) => set({ currentChild: child }),

  clearChild: () => set({ currentChild: null }),
}));