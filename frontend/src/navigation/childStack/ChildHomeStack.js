import { createStackNavigator } from "@react-navigation/stack";
import childExpenseLogScreen from "../../components/screens/childScreen/childExpenseLogScreen/childExpenseLogScreen";
import ChildGoalScreen from "../../components/screens/childScreen/childGoalScreen/ChildGoalScreen";
import ChildHomeScreen from "../../components/screens/childScreen/childHomeScreen/ChildHomeScreen";

const stack=createStackNavigator();

export default function ChildHomeStack(){
    return(
    <stack.Navigator screenOptions={{headerShown:false}}>
        <stack.Screen name="ChildHomeScreen" component={ChildHomeScreen}/>
        <stack.Screen name="childExpenseLogScreen" component={childExpenseLogScreen}/>
        <stack.Screen name="ChildGoalScreen" component={ChildGoalScreen}/>
    </stack.Navigator>
    )
}