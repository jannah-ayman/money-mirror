import { createStackNavigator } from "@react-navigation/stack";
import ParentProfileScreen from "../../components/screens/parentScreen/parentProfileScreen/ParentProfileScreen";
import EditProfileScreen from "../../components/screens/parentScreen/parentProfileScreen/EditProfileScreen";
import ChangePasswordScreen from "../../components/screens/parentScreen/parentProfileScreen/ChangePasswordScreen";
import ChangeEmailScreen from "../../components/screens/parentScreen/parentProfileScreen/Changeemailscreen";
import NotificationSettingsScreen from "../../components/screens/parentScreen/parentProfileScreen/NotificationSettingsScreen";
import AppSettingsScreen from "../../components/screens/parentScreen/parentProfileScreen/AppSettingsScreen";
import HelpSupportScreen from "../../components/screens/parentScreen/parentProfileScreen/HelpSupportScreen";


const Stack = createStackNavigator();

export default function ParentProfileStack() {
    return (
        <Stack.Navigator 
            initialRouteName="ParentProfile"
            screenOptions={{
                headerShown: false,
                cardStyleInterpolator: ({ current, layouts }) => {
                    return {
                        cardStyle: {
                            transform: [
                                {
                                    translateX: current.progress.interpolate({
                                        inputRange: [0, 1],
                                        outputRange: [layouts.screen.width, 0],
                                    }),
                                },
                            ],
                        },
                    };
                },
            }}
        >
            <Stack.Screen 
                name="ParentProfile" 
                component={ParentProfileScreen}
                options={{
                    title: 'Profile'
                }}
            />
            
            <Stack.Screen 
                name="EditProfileScreen" 
                component={EditProfileScreen}
                options={{
                    title: 'Edit Profile Details'
                }}
            />
            
            <Stack.Screen 
                name="ChangePassword" 
                component={ChangePasswordScreen}
                options={{
                    title: 'Change Password'
                }}
            />
            
            <Stack.Screen 
                name="ChangeEmailScreen"
                component={ChangeEmailScreen}
                options={{
                    title: 'Change Email'
                }}
            />
            
            <Stack.Screen 
                name="NotificationSettings" 
                component={NotificationSettingsScreen}
                options={{
                    title: 'Notifications'
                }}
            />
            
            <Stack.Screen 
                name="AppSettings" 
                component={AppSettingsScreen}
                options={{
                    title: 'Settings'
                }}
            />
            
            <Stack.Screen 
                name="HelpSupport" 
                component={HelpSupportScreen}
                options={{
                    title: 'Help & Support'
                }}
            />
        </Stack.Navigator>
    );
}