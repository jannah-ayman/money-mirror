import { useFonts } from 'expo-font';

export default function useAppFonts() {
  const [fontsLoaded] = useFonts({
    'MouseMemoirs-Regular': require('../../assets/fonts/MouseMemoirs-Regular.ttf'),
  });

  return fontsLoaded;
}
