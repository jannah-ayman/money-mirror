import { NavigationContainer } from '@react-navigation/native';
import MainStack from './src/navigation/MainStack';
import useAppFonts from './src/config/fonts';
import Toast from 'react-native-toast-message';
import { MyTheme } from './Theme';


export default function App() {
  const fontsLoaded = useAppFonts();
  if (!fontsLoaded) {
    return null;
  }else{
    return (
      <NavigationContainer theme={MyTheme}>
        <MainStack />
        <Toast/>
      </NavigationContainer>
    );
  }

}

