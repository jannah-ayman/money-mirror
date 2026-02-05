import { createStackNavigator } from "@react-navigation/stack";
import ParentSignUp from "../../components/screens/parentScreen/parentSignUp/ParentSignUp";
import ParentLogin from "../../components/screens/parentScreen/parentLogin/ParentLogin";
import ParentMainTabs from "../parentTabs/ParentMainTabs";

const stack=createStackNavigator();

export default function ParentStack() {
  return (
    <stack.Navigator screenOptions={{headerShown:false}}>
        <stack.Screen name="ParentSignUp" component={ParentSignUp}/>
        <stack.Screen name="ParentLogin" component={ParentLogin}/>
        <stack.Screen name="ParentMainTabs" component={ParentMainTabs}/>
          

    </stack.Navigator>
  )};