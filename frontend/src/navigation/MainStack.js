import { createStackNavigator } from "@react-navigation/stack";
import SplashScreen from "../components/screens/splash/SplashScreen";
import IntroScreen from "../components/screens/intro/IntroScreen";
import ParentStack from "./parentStack/ParentStack";
import ChildStack from "./childStack/ChildStack"


const stack = createStackNavigator();

export default function MainStack() {
  return (
    <stack.Navigator screenOptions={{headerShown: false}}>
      <stack.Screen name="Splash" component={SplashScreen} />
      <stack.Screen name="Intro" component={IntroScreen} />
      <stack.Screen name="ParentStack" component={ParentStack} />
      <stack.Screen name="ChildStack" component={ChildStack} />
    </stack.Navigator>
  );
}