import { createStackNavigator } from "@react-navigation/stack"
import ChildLogin from "../../components/screens/childScreen/childLogin/ChildLogin";
import ChildMainTabs from "../childTabs/ChildMainTabs"

const stack=createStackNavigator();

export default function ChildStack(){
    return(
        <stack.Navigator screenOptions={{headerShown:false}}>
            <stack.Screen name="ChildLogin" component={ChildLogin}/>
            <stack.Screen name="ChildMainTabs" component={ChildMainTabs}/>
        </stack.Navigator>
    )
}