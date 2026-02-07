import { createBottomTabNavigator } from "@react-navigation/bottom-tabs";
import { Ionicons } from '@expo/vector-icons';
import { View, StyleSheet, Platform } from 'react-native';
import { LinearGradient } from 'expo-linear-gradient';
import ParentHomeStack from "../parentStack/ParentHomeStack";
import ParentExpenseScreen from "../../components/screens/parentScreen/parentExpenseScreen/ParentExpenseScreen";
import ParentAnalysisScreen from "../../components/screens/parentScreen/parentAnalysisScreen/ParentAnalysisScreen";
import ParentReportScreen from "../../components/screens/parentScreen/parentReportScreen/ParentReportScreen";
import ParentProfileScreen from "../../components/screens/parentScreen/parentProfileScreen/ParentProfileScreen";

const tabs = createBottomTabNavigator();

export default function ParentMainTabs() {
  return (
    <tabs.Navigator
      screenOptions={({ route }) => ({
        headerShown: false,
        tabBarIcon: ({ focused, color, size }) => {
          let iconName;

          if (route.name === 'ParentHomeStack') {
            iconName = focused ? 'home' : 'home-outline';
          } else if (route.name === 'ParentExpenseScreen') {
            iconName = focused ? 'wallet' : 'wallet-outline';
          } else if (route.name === 'ParentAnalysisScreen') {
            iconName = focused ? 'bar-chart' : 'bar-chart-outline';
          } else if (route.name === 'ParentReportScreen') {
            iconName = focused ? 'document-text' : 'document-text-outline';
          } else if (route.name === 'ParentProfileScreen') {
            iconName = focused ? 'person' : 'person-outline';
          }

          if (focused) {
            return (
              <View style={styles.activeTabContainer}>
                <View style={styles.activeTabGlow} />
                <LinearGradient
                  colors={['#5F3BFF', '#3B1DFF', '#7F3FFF']}                  start={{ x: 0, y: 0 }}
                  end={{ x: 1, y: 1 }}
                  style={styles.activeTabGradient}
                >
                  <Ionicons name={iconName} size={28} color="#FFFFFF" />
                </LinearGradient>
              </View>
            );
          }

          return (
            <View style={styles.inactiveTabIcon}>
              <Ionicons name={iconName} size={25} color={color} />
            </View>
          );
        },
          tabBarActiveTintColor: '#6366F1',
          tabBarInactiveTintColor: '#A3AED0',  
         tabBarShowLabel: true,
          tabBarLabelStyle: {
          fontSize: 11,
          fontWeight: '700',
          marginTop: 4,
          marginBottom: Platform.OS === 'ios' ? 0 : 10,
          letterSpacing: 0.5,
          textTransform: 'capitalize',
        },
        tabBarStyle: {
          backgroundColor: '#F4F6FB',          borderTopLeftRadius: 28,
          borderTopRightRadius: 28,
          height: Platform.OS === 'ios' ? 92 : 75,
          paddingTop: 10,
          paddingBottom: Platform.OS === 'ios' ? 24 : 10,
          paddingHorizontal: 12,
          position: 'absolute',
          borderTopWidth: 0,
          elevation: 35,
          shadowColor: '#0F172A',
          shadowOffset: { width: 0, height: -10 },
          shadowOpacity: 0.15,
          shadowRadius: 25,
        },
        tabBarItemStyle: {
          paddingVertical: 6,
          marginHorizontal: 2,
        },
      })}
    >
      <tabs.Screen 
        name="ParentHomeStack" 
        component={ParentHomeStack}
        options={{ tabBarLabel: 'Home' }}
      />
      <tabs.Screen 
        name="ParentExpenseScreen" 
        component={ParentExpenseScreen}
        options={{ tabBarLabel: 'Expense' }}
      />
      <tabs.Screen 
        name="ParentAnalysisScreen" 
        component={ParentAnalysisScreen}
        options={{ tabBarLabel: 'Analysis' }}
      />
      <tabs.Screen 
        name="ParentReportScreen" 
        component={ParentReportScreen}
        options={{ tabBarLabel: 'Report' }}
      />
      <tabs.Screen 
        name="ParentProfileScreen" 
        component={ParentProfileScreen}
        options={{ tabBarLabel: 'Profile' }}
      />
    </tabs.Navigator>
  );
}

const styles = StyleSheet.create({
  // ========== ACTIVE TAB ========== //
  activeTabContainer: {
    marginTop: -28,
    alignItems: 'center',
    justifyContent: 'center',
    position: 'relative',
  },
  
  activeTabGlow: {
    position: 'absolute',
    width: 76,
    height: 76,
    borderRadius: 38,
    backgroundColor: 'rgba(95, 59, 255, 0.25)', 
    shadowColor: '#7F3FFF', 
    shadowOffset: { width: 0, height: 0 },
    shadowOpacity: 0.5,
    shadowRadius: 25,
    elevation: 8,
  },
  
  activeTabGradient: {
    width: 64,
    height: 64,
    borderRadius: 32,
    justifyContent: 'center',
    alignItems: 'center',
    shadowColor: '#6366F1',
    shadowOffset: { width: 0, height: 8 },
    shadowOpacity: 0.5,
    shadowRadius: 20,
    elevation: 15,
    borderWidth: 4,
    borderColor: '#FFFFFF',
  },

  // ========== INACTIVE TAB ========== //
  inactiveTabIcon: {
    width: 48,
    height: 48,
    borderRadius: 24,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: 'transparent',
  },
});

