// import { createBottomTabNavigator } from "@react-navigation/bottom-tabs";
// import ChildHomeStack from "../childStack/ChildHomeStack"
// import ChildHistoryScreen from "../../components/screens/childScreen/childHistoryScreen/ChildHistoryScreen"
// import ChildQuizScreen from "../../components/screens/childScreen/childQuizScreen/ChildQuizScreen"
// import ChildBadgesScreen from "../../components/screens/childScreen/childBadgesScreen/ChildBadgesScreen"
// import ChildProfileScreen from "../../components/screens/childScreen/childProfileScreen/ChildProfileScreen"
// // import Feather from '@expo/vector-icons/Feather';
// import { Image } from "react-native";

// const tabs=createBottomTabNavigator();

// export default function ChildMainTabs(){
//     return(
//         <tabs.Navigator screenOptions={{
//             headerShown:false,
//             tabBarStyle: {
//             backgroundColor: '#2B235A',
//             height: 80,
//             borderTopLeftRadius: 20,
//             borderTopRightRadius: 20,
//             position: 'absolute',
//             paddingTop: 10,
//             marginBottom:-1,
//             },
//             tabBarLabelStyle: {
//             fontSize: 14,
//             marginBottom: 5,
//             },
//             tabBarActiveTintColor: '#bc96ff',
//             tabBarInactiveTintColor: '#ffffff',
//         }}>
//             <tabs.Screen name="ChildHomeStack" component={ChildHomeStack}
//                 options={{
//                 tabBarLabel:"Home",
//                 tabBarIcon:() => (
//                     <Image
//                         source={require('../../../assets/images/3d-house.png')}
//                         style={{
//                         width: 26,
//                         height: 26,
//                         resizeMode: 'contain',
//                         }}
//                     />
//                     ),
//                 }}
//             />
//             <tabs.Screen name="ChildHistoryScreen" component={ChildHistoryScreen}
//                 options={{
//                 tabBarLabel:"History",
//                 tabBarIcon:() => (
//                     <Image
//                         source={require('../../../assets/images/history-book.png')}
//                         style={{
//                         width: 26,
//                         height: 26,
//                         resizeMode: 'contain',
//                         }}
//                     />
//                     ),
//                 }}
//             />
//             <tabs.Screen name="ChildQuizScreen" component={ChildQuizScreen}
//                 options={{
//                 tabBarLabel:"Quiz",
//                 tabBarIcon:() => (
//                     <Image
//                         source={require('../../../assets/images/quiz.png')}
//                         style={{
//                         width: 26,
//                         height: 26,
//                         resizeMode: 'contain',
//                         }}
//                     />
//                     ),
//                 }}
//             />
//             <tabs.Screen name="ChildBadgesScreen" component={ChildBadgesScreen}
//                 options={{
//                 tabBarLabel:"Badges",
//                 tabBarIcon:() => (
//                     <Image
//                         source={require('../../../assets/images/rewards.png')}
//                         style={{
//                         width: 26,
//                         height: 26,
//                         resizeMode: 'contain',
//                         }}
//                     />
//                     ),
//                 }}
//             />
//             <tabs.Screen name="ChildProfileScreen" component={ChildProfileScreen}
//                 options={{
//                 tabBarLabel:"profile",
//                 tabBarIcon:() => (
//                     <Image
//                         source={require('../../../assets/images/astronaut (2).png')}
//                         style={{
//                         width: 26,
//                         height: 26,
//                         resizeMode: 'contain',
//                         }}
//                     />
//                     ),
//                 }}
//             />
//         </tabs.Navigator>
//     );
// }


import { createBottomTabNavigator } from "@react-navigation/bottom-tabs";
import { View, Image } from "react-native";

import ChildHomeStack from "../childStack/ChildHomeStack";
import ChildHistoryScreen from "../../components/screens/childScreen/childHistoryScreen/ChildHistoryScreen";
import ChildQuizScreen from "../../components/screens/childScreen/childQuizScreen/ChildQuizScreen";
import ChildBadgesScreen from "../../components/screens/childScreen/childBadgesScreen/ChildBadgesScreen";
import ChildProfileScreen from "../../components/screens/childScreen/childProfileScreen/ChildProfileScreen";

const Tabs = createBottomTabNavigator();

/* ---------- Icon Component ---------- */
const TabIcon = ({ focused, source }) => {
  return (
    <View
      style={{
        width: 50,
        height: 50,
        borderRadius: 26,
        backgroundColor: focused ? "#7d6cff" : "transparent",
        justifyContent: "center",
        alignItems: "center",
        marginTop: focused ? -20 : 0,
        shadowColor: focused ? "#7d6cff" : "transparent",
        shadowOffset: { width: 0, height: 6 },
        shadowOpacity: 0.4,
        shadowRadius: 8,
        elevation: focused ? 10 : 0,
      }}
    >
      <Image
        source={source}
        style={{
          width: focused ? 30 : 26,
          height: focused ? 30 : 26,
          resizeMode: "contain",
        }}
      />
    </View>
  );
};

export default function ChildMainTabs() {
  return (
    <Tabs.Navigator
      screenOptions={{
        headerShown: false,
        tabBarStyle: {
          backgroundColor: "#0F172A",
        //   backgroundColor: "#0F172A",
          height: 88,
          borderTopLeftRadius: 30,
          borderTopRightRadius: 30,
          position: "absolute",
          paddingTop: 12,
          paddingBottom: 10,
          shadowColor: "#000",
          shadowOffset: { width: 0, height: -6 },
          shadowOpacity: 0.3,
          shadowRadius: 12,
          elevation: 25,
        },
        tabBarLabelStyle: {
          fontSize: 14,
          fontWeight: "600",
          paddingtop:20,
        },
        tabBarActiveTintColor: "#b6acff",
        tabBarInactiveTintColor: "#fff",
      }}
    >
      <Tabs.Screen
        name="ChildHomeStack"
        component={ChildHomeStack}
        options={{
          tabBarLabel: "Home",
          tabBarIcon: ({ focused }) => (
            <TabIcon
              focused={focused}
              source={require("../../../assets/images/3d-house.png")}
            />
          ),
        }}
      />

      <Tabs.Screen
        name="ChildHistoryScreen"
        component={ChildHistoryScreen}
        options={{
          tabBarLabel: "History",
          tabBarIcon: ({ focused }) => (
            <TabIcon
              focused={focused}
              source={require("../../../assets/images/history-book.png")}
            />
          ),
        }}
      />

      <Tabs.Screen
        name="ChildQuizScreen"
        component={ChildQuizScreen}
        options={{
          tabBarLabel: "Quiz",
          tabBarIcon: ({ focused }) => (
            <TabIcon
              focused={focused}
              source={require("../../../assets/images/quiz.png")}
            />
          ),
        }}
      />

      <Tabs.Screen
        name="ChildBadgesScreen"
        component={ChildBadgesScreen}
        options={{
          tabBarLabel: "Badges",
          tabBarIcon: ({ focused }) => (
            <TabIcon
              focused={focused}
              source={require("../../../assets/images/rewards.png")}
            />
          ),
        }}
      />

      <Tabs.Screen
        name="ChildProfileScreen"
        component={ChildProfileScreen}
        options={{
          tabBarLabel: "Profile",
          tabBarIcon: ({ focused }) => (
            <TabIcon
              focused={focused}
              source={require("../../../assets/images/astronaut (2).png")}
            />
          ),
        }}
      />
    </Tabs.Navigator>
  );
}
