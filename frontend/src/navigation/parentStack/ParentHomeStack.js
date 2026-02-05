import { createStackNavigator } from "@react-navigation/stack";
import ParentHomeScreen from "../../components/screens/parentScreen/parentHomeScreen/ParentHomeScreen"
import ParentAllowanceScreen from "../../components/screens/parentScreen/parentAllowanceScren/ParentAllowanceScren"
import ParentGoalScreen from "../../components/screens/parentScreen/parentGoalScreen/ParentGoalScreen"
import ParentProfileScreen from "../../components/screens/parentScreen/parentProfileScreen/ParentProfileScreen";
import ManageChildrenScreen from "../../components/screens/parentScreen/parentProfileScreen/Managechildrenscreen";
import AddNewChildScreen from "../../components/screens/parentScreen/parentProfileScreen/Addnewchildscreen";
import InitialProfilingScreen from "../../components/screens/parentScreen/parentProfileScreen/Initialprofilingscreen";
import ChangeEmailScreen from "../../components/screens/parentScreen/parentProfileScreen/Changeemailscreen";
import EditProfileScreen from "../../components/screens/parentScreen/parentProfileScreen/EditProfileScreen";
import ChangePasswordScreen from "../../components/screens/parentScreen/parentProfileScreen/Changepasswordscreen";
import NotificationSettingsScreen from "../../components/screens/parentScreen/parentProfileScreen/Notificationsettingsscreen";
import AppSettingsScreen from "../../components/screens/parentScreen/parentProfileScreen/Appsettingsscreen";
import HelpSupportScreen from "../../components/screens/parentScreen/parentProfileScreen/Helpsupportscreen";
import NotificationsScreen from "../../components/screens/parentScreen/parentHomeScreen/Notificationsscreen";

const stack = createStackNavigator();

export default function ParentHomeStack(){
    return(
        <stack.Navigator screenOptions={{headerShown:false}}>
            <stack.Screen name="ParentHomeScreen" component={ParentHomeScreen}/>
            <stack.Screen name="ParentAllowanceScreen" component={ParentAllowanceScreen}/>
            <stack.Screen name="ParentGoalScreen" component={ParentGoalScreen}/>
            <stack.Screen name="ManageChildren" component={ManageChildrenScreen} />
            <stack.Screen name="AddNewChild" component={AddNewChildScreen} />
            <stack.Screen name="InitialProfiling" component={InitialProfilingScreen} />
            <stack.Screen name="ParentProfileScreen" component={ParentProfileScreen}/>
            <stack.Screen name="ChangeEmailScreen" component={ChangeEmailScreen}/>
            <stack.Screen name="EditProfileScreen" component={EditProfileScreen}/>
            <stack.Screen name="ChangePasswordScreen" component={ChangePasswordScreen}/>
            <stack.Screen name="NotificationSettingsScreen" component={NotificationSettingsScreen}/>
            <stack.Screen name="AppSettingsScreen" component={AppSettingsScreen}/> 
            <stack.Screen name="HelpSupportScreen" component={HelpSupportScreen}/>
            <stack.Screen name="NotificationsScreen" component={NotificationsScreen}/>
        </stack.Navigator>
    )
}