import React, { useState } from 'react'
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  StatusBar,
  Switch,
  Modal,
  Animated
} from 'react-native'

import { useNavigation } from '@react-navigation/native'
import { Ionicons } from '@expo/vector-icons'
import { LinearGradient } from 'expo-linear-gradient'

export default function NotificationSettingsScreen() {
  const navigation = useNavigation()
  
  // Notification States
  const [pushNotifications, setPushNotifications] = useState(true)
  const [emailNotifications, setEmailNotifications] = useState(true)
  
  // Expense Notifications
  const [expenseAlerts, setExpenseAlerts] = useState(true)
  const [dailyExpenseSummary, setDailyExpenseSummary] = useState(false)
  const [weeklyExpenseReport, setWeeklyExpenseReport] = useState(true)
  
  // Goal Notifications
  const [goalProgress, setGoalProgress] = useState(true)
  const [goalAchieved, setGoalAchieved] = useState(true)
  const [goalReminders, setGoalReminders] = useState(false)
  
  // Child Activity Notifications
  const [childLogin, setChildLogin] = useState(false)
  const [childPurchase, setChildPurchase] = useState(true)
  const [childMilestone, setChildMilestone] = useState(true)
  
  // Profile & Account
  const [profileUpdates, setProfileUpdates] = useState(false)
  const [securityAlerts, setSecurityAlerts] = useState(true)
  const [allowanceReminders, setAllowanceReminders] = useState(true)

  const [showSuccessModal, setShowSuccessModal] = useState(false)
  const scaleAnim = useState(new Animated.Value(0))[0]

  const handleSaveSettings = () => {
    showSuccessMessage()
    
    // Here you would save to API/database
    // saveNotificationSettings({
    //   pushNotifications,
    //   emailNotifications,
    //   expenseAlerts,
    //   ...
    // })
  }

  const showSuccessMessage = () => {
    setShowSuccessModal(true)
    Animated.spring(scaleAnim, {
      toValue: 1,
      tension: 50,
      friction: 7,
      useNativeDriver: true,
    }).start()

    setTimeout(() => {
      closeSuccessModal()
    }, 1500)
  }

  const closeSuccessModal = () => {
    Animated.timing(scaleAnim, {
      toValue: 0,
      duration: 200,
      useNativeDriver: true,
    }).start(() => {
      setShowSuccessModal(false)
    })
  }

  const renderNotificationItem = (title, description, value, onValueChange, icon, iconColor) => {
    return (
      <View style={styles.notificationItem}>
        <View style={styles.notificationLeft}>
          <View style={[styles.iconContainer, { backgroundColor: `${iconColor}15` }]}>
            <Ionicons name={icon} size={22} color={iconColor} />
          </View>
          <View style={styles.notificationText}>
            <Text style={styles.notificationTitle}>{title}</Text>
            {description && (
              <Text style={styles.notificationDescription}>{description}</Text>
            )}
          </View>
        </View>
        <Switch
          value={value}
          onValueChange={onValueChange}
          trackColor={{ false: '#E6E8F0', true: '#A78BFA' }}
          thumbColor={value ? '#5F3BFF' : '#f4f3f4'}
          ios_backgroundColor="#E6E8F0"
        />
      </View>
    )
  }

  return (
    <View style={styles.container}>
      <StatusBar barStyle="light-content" />

      {/* Header */}
      <LinearGradient
        colors={['#1F1147', '#2B1055', '#4B0082', '#5F3BFF']}
        start={{ x: 0, y: 0 }}
        end={{ x: 1, y: 1 }}
        style={styles.header}
      >
        <View style={styles.headerContent}>
          <TouchableOpacity 
            style={styles.backButton}
            onPress={() => navigation.goBack()}
          >
            <Ionicons name="arrow-back" size={24} color="#fff" />
          </TouchableOpacity>

          <View style={styles.headerTitleContainer}>
            <Text style={styles.headerTitle}>Notifications</Text>
            <Text style={styles.headerSubtitle}>Manage your preferences</Text>
          </View>

          <View style={{ width: 40 }} />
        </View>
      </LinearGradient>

      <ScrollView 
        style={styles.scrollView}
        contentContainerStyle={styles.scrollContent}
        showsVerticalScrollIndicator={false}
      >
        {/* General Settings */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>General</Text>
          <View style={styles.card}>
            {renderNotificationItem(
              'Push Notifications',
              'Receive notifications on your device',
              pushNotifications,
              setPushNotifications,
              'notifications',
              '#5F3BFF'
            )}
            <View style={styles.divider} />
            {renderNotificationItem(
              'Email Notifications',
              'Receive updates via email',
              emailNotifications,
              setEmailNotifications,
              'mail',
              '#5F3BFF'
            )}
          </View>
        </View>

        {/* Expense Notifications */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Expense Alerts</Text>
          <View style={styles.card}>
            {renderNotificationItem(
              'Expense Alerts',
              'When your child logs a purchase',
              expenseAlerts,
              setExpenseAlerts,
              'cash',
              '#00E0C6'
            )}
            <View style={styles.divider} />
            {renderNotificationItem(
              'Daily Summary',
              'Daily expense recap',
              dailyExpenseSummary,
              setDailyExpenseSummary,
              'calendar',
              '#00E0C6'
            )}
            <View style={styles.divider} />
            {renderNotificationItem(
              'Weekly Report',
              'Comprehensive weekly analysis',
              weeklyExpenseReport,
              setWeeklyExpenseReport,
              'stats-chart',
              '#00E0C6'
            )}
          </View>
        </View>

        {/* Goal Notifications */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Goals & Challenges</Text>
          <View style={styles.card}>
            {renderNotificationItem(
              'Goal Progress',
              'Updates on savings goal progress',
              goalProgress,
              setGoalProgress,
              'trophy',
              '#FFD700'
            )}
            <View style={styles.divider} />
            {renderNotificationItem(
              'Goal Achieved',
              'When a goal is completed',
              goalAchieved,
              setGoalAchieved,
              'checkmark-circle',
              '#FFD700'
            )}
            <View style={styles.divider} />
            {renderNotificationItem(
              'Goal Reminders',
              'Reminders to contribute to goals',
              goalReminders,
              setGoalReminders,
              'time',
              '#FFD700'
            )}
          </View>
        </View>

        {/* Child Activity */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Child Activity</Text>
          <View style={styles.card}>
            {renderNotificationItem(
              'Login Activity',
              'When your child logs in',
              childLogin,
              setChildLogin,
              'log-in',
              '#FF6B9D'
            )}
            <View style={styles.divider} />
            {renderNotificationItem(
              'Purchase Activity',
              'Real-time purchase notifications',
              childPurchase,
              setChildPurchase,
              'cart',
              '#FF6B9D'
            )}
            <View style={styles.divider} />
            {renderNotificationItem(
              'Milestones',
              'Achievement and badge unlocks',
              childMilestone,
              setChildMilestone,
              'star',
              '#FF6B9D'
            )}
          </View>
        </View>

        {/* Account & Security */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Account & Security</Text>
          <View style={styles.card}>
            {renderNotificationItem(
              'Profile Updates',
              'Changes to your profile',
              profileUpdates,
              setProfileUpdates,
              'person-circle',
              '#2A236F'
            )}
            <View style={styles.divider} />
            {renderNotificationItem(
              'Security Alerts',
              'Unusual account activity',
              securityAlerts,
              setSecurityAlerts,
              'shield-checkmark',
              '#2A236F'
            )}
            <View style={styles.divider} />
            {renderNotificationItem(
              'Allowance Reminders',
              'When it\'s time to set allowance',
              allowanceReminders,
              setAllowanceReminders,
              'wallet',
              '#2A236F'
            )}
          </View>
        </View>

        {/* Info Notice */}
        <View style={styles.infoNotice}>
          <Ionicons name="information-circle" size={20} color="#5F3BFF" />
          <Text style={styles.infoText}>
            You can change these settings anytime. Critical security alerts cannot be disabled.
          </Text>
        </View>

        {/* Save Button */}
        <TouchableOpacity 
          style={styles.saveButton}
          onPress={handleSaveSettings}
          activeOpacity={0.8}
        >
          <LinearGradient
            colors={['#5F3BFF', '#3B1DFF']}
            start={{ x: 0, y: 0 }}
            end={{ x: 1, y: 0 }}
            style={styles.saveGradient}
          >
            <Ionicons name="checkmark-circle" size={20} color="#fff" />
            <Text style={styles.saveButtonText}>Save Preferences</Text>
          </LinearGradient>
        </TouchableOpacity>
      </ScrollView>

      {/* Custom Success Modal */}
      <Modal
        transparent={true}
        visible={showSuccessModal}
        animationType="none"
        onRequestClose={closeSuccessModal}
      >
        <View style={styles.modalOverlay}>
          <Animated.View 
            style={[
              styles.successCard,
              { transform: [{ scale: scaleAnim }] }
            ]}
          >
            <LinearGradient
              colors={['#00D4AA', '#00B894']}
              start={{ x: 0, y: 0 }}
              end={{ x: 1, y: 1 }}
              style={styles.successIconContainer}
            >
              <Ionicons name="notifications" size={60} color="#fff" />
            </LinearGradient>
            
            <Text style={styles.successTitle}>Saved!</Text>
            <Text style={styles.successMessage}>
              Your notification preferences have been updated
            </Text>

            <View style={styles.successCheck}>
              <Ionicons name="checkmark" size={24} color="#00D4AA" />
            </View>
          </Animated.View>
        </View>
      </Modal>
    </View>
  )
}

const styles = StyleSheet.create({
  /* ================= BASE ================= */
  container: {
    flex: 1,
    backgroundColor: '#F4F6FB',
  },

  /* ================= HEADER ================= */
  header: {
    paddingTop: StatusBar.currentHeight ? StatusBar.currentHeight + 15 : 55,
    paddingBottom: 24,
    borderBottomLeftRadius: 30,
    borderBottomRightRadius: 30,
    shadowColor: '#3B1DFF',
    shadowOffset: { width: 0, height: 8 },
    shadowOpacity: 0.7,
    shadowRadius: 25,
    elevation: 10,
  },

  headerContent: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: 24,
  },

  backButton: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    justifyContent: 'center',
    alignItems: 'center',
  },

  headerTitleContainer: {
    alignItems: 'center',
    flex: 1,
  },

  headerTitle: {
    color: '#fff',
    fontSize: 22,
    fontWeight: '800',
    letterSpacing: 0.5,
  },

  headerSubtitle: {
    color: 'rgba(255, 255, 255, 0.8)',
    fontSize: 14,
    fontWeight: '500',
    marginTop: 2,
  },

  /* ================= SCROLL VIEW ================= */
  scrollView: {
    flex: 1,
  },

  scrollContent: {
    padding: 24,
    paddingBottom: 40,
  },

  /* ================= SECTIONS ================= */
  section: {
    marginBottom: 24,
  },

  sectionTitle: {
    color: '#231c63',
    fontSize: 18,
    fontWeight: '800',
    marginBottom: 12,
  },

  card: {
    backgroundColor: '#FFFFFF',
    borderRadius: 16,
    padding: 4,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 8,
    elevation: 2,
  },

  /* ================= NOTIFICATION ITEM ================= */
  notificationItem: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingVertical: 12,
    paddingHorizontal: 16,
  },

  notificationLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
    marginRight: 16,
  },

  iconContainer: {
    width: 44,
    height: 44,
    borderRadius: 12,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 12,
  },

  notificationText: {
    flex: 1,
  },

  notificationTitle: {
    color: '#231c63',
    fontSize: 16,
    fontWeight: '700',
    marginBottom: 2,
  },

  notificationDescription: {
    color: '#8A8FB0',
    fontSize: 13,
    fontWeight: '500',
    lineHeight: 18,
  },

  divider: {
    height: 1,
    backgroundColor: '#F0F0F5',
    marginLeft: 72,
  },

  /* ================= INFO NOTICE ================= */
  infoNotice: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#E8E1FF',
    padding: 14,
    borderRadius: 12,
    marginBottom: 24,
  },

  infoText: {
    flex: 1,
    marginLeft: 12,
    color: '#5F3BFF',
    fontSize: 13,
    fontWeight: '600',
    lineHeight: 18,
  },

  /* ================= SAVE BUTTON ================= */
  saveButton: {
    borderRadius: 16,
    overflow: 'hidden',
    shadowColor: '#5F3BFF',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 12,
    elevation: 6,
    marginBottom: 50,
  },

  saveGradient: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 16,
    paddingHorizontal: 24,
  },

  saveButtonText: {
    color: '#FFFFFF',
    fontSize: 16,
    fontWeight: '800',
    marginLeft: 8,
  },

  /* ================= SUCCESS MODAL ================= */
  modalOverlay: {
    flex: 1,
    backgroundColor: 'rgba(0, 0, 0, 0.5)',
    justifyContent: 'center',
    alignItems: 'center',
    padding: 24,
  },

  successCard: {
    backgroundColor: '#FFFFFF',
    borderRadius: 24,
    padding: 32,
    alignItems: 'center',
    width: '100%',
    maxWidth: 320,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 10 },
    shadowOpacity: 0.3,
    shadowRadius: 20,
    elevation: 10,
  },

  successIconContainer: {
    width: 100,
    height: 100,
    borderRadius: 50,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 24,
    shadowColor: '#00D4AA',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.4,
    shadowRadius: 12,
    elevation: 8,
  },

  successTitle: {
    fontSize: 28,
    fontWeight: '800',
    color: '#231c63',
    marginBottom: 12,
  },

  successMessage: {
    fontSize: 16,
    fontWeight: '600',
    color: '#8A8FB0',
    textAlign: 'center',
    lineHeight: 24,
    marginBottom: 24,
  },

  successCheck: {
    width: 48,
    height: 48,
    borderRadius: 24,
    backgroundColor: '#E8F9F6',
    justifyContent: 'center',
    alignItems: 'center',
  },
});