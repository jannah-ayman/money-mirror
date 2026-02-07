import React, { useState } from 'react'
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  TextInput,
  ScrollView,
  StatusBar,
  Alert,
  KeyboardAvoidingView,
  Platform,
  Modal,
  Animated
} from 'react-native'

import { useNavigation } from '@react-navigation/native'
import { Ionicons } from '@expo/vector-icons'
import { LinearGradient } from 'expo-linear-gradient'

export default function ChangePasswordScreen() {
  const navigation = useNavigation()
  
  const [currentPassword, setCurrentPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  
  const [showCurrentPassword, setShowCurrentPassword] = useState(false)
  const [showNewPassword, setShowNewPassword] = useState(false)
  const [showConfirmPassword, setShowConfirmPassword] = useState(false)
  
  const [isLoading, setIsLoading] = useState(false)
  const [showSuccessModal, setShowSuccessModal] = useState(false)
  const scaleAnim = useState(new Animated.Value(0))[0]

  // Password strength checker
  const getPasswordStrength = (password) => {
    if (password.length === 0) return { strength: 0, label: '', color: '#E6E8F0' }
    if (password.length < 6) return { strength: 1, label: 'Weak', color: '#FF6B9D' }
    if (password.length < 8) return { strength: 2, label: 'Fair', color: '#FFB800' }
    if (password.length >= 8 && /[A-Z]/.test(password) && /[0-9]/.test(password)) {
      return { strength: 3, label: 'Strong', color: '#00D4AA' }
    }
    return { strength: 2, label: 'Good', color: '#5F3BFF' }
  }

  const passwordStrength = getPasswordStrength(newPassword)

  const handleChangePassword = () => {
    // Validation
    if (!currentPassword.trim()) {
      Alert.alert('Error', 'Please enter your current password')
      return
    }

    if (!newPassword.trim()) {
      Alert.alert('Error', 'Please enter a new password')
      return
    }

    if (newPassword.length < 6) {
      Alert.alert('Error', 'Password must be at least 6 characters long')
      return
    }

    if (newPassword === currentPassword) {
      Alert.alert('Error', 'New password must be different from current password')
      return
    }

    if (newPassword !== confirmPassword) {
      Alert.alert('Error', 'New passwords do not match')
      return
    }

    // Confirm change
    Alert.alert(
      'Change Password',
      'Are you sure you want to change your password?',
      [
        {
          text: 'Cancel',
          style: 'cancel'
        },
        {
          text: 'Change',
          onPress: processPasswordChange
        }
      ]
    )
  }

  const processPasswordChange = async () => {
    setIsLoading(true)

    // Simulate API call
    setTimeout(() => {
      setIsLoading(false)
      showSuccessMessage()

      // Here you would call your API:
      // try {
      //   const response = await changePasswordAPI(currentPassword, newPassword)
      //   if (response.success) {
      //     showSuccessMessage()
      //   }
      // } catch (error) {
      //   Alert.alert('Error', error.message)
      // }
    }, 1500)
  }

  const showSuccessMessage = () => {
    setShowSuccessModal(true)
    Animated.spring(scaleAnim, {
      toValue: 1,
      tension: 50,
      friction: 7,
      useNativeDriver: true,
    }).start()

    // Auto close after 2 seconds
    setTimeout(() => {
      closeSuccessModal()
    }, 2000)
  }

  const closeSuccessModal = () => {
    Animated.timing(scaleAnim, {
      toValue: 0,
      duration: 200,
      useNativeDriver: true,
    }).start(() => {
      setShowSuccessModal(false)
      navigation.goBack()
    })
  }

  return (
    <KeyboardAvoidingView 
      style={styles.container}
      behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
    >
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
            <Text style={styles.headerTitle}>Change Password</Text>
            <Text style={styles.headerSubtitle}>Update your password</Text>
          </View>

          <View style={{ width: 40 }} />
        </View>
      </LinearGradient>

      <ScrollView 
        style={styles.scrollView}
        contentContainerStyle={styles.scrollContent}
        showsVerticalScrollIndicator={false}
        keyboardShouldPersistTaps="handled"
      >
        {/* Security Info Card */}
        <View style={styles.infoCard}>
          <View style={styles.infoIconContainer}>
            <Ionicons name="shield-checkmark" size={24} color="#5F3BFF" />
          </View>
          <View style={styles.infoTextContainer}>
            <Text style={styles.infoTitle}>Security Reminder</Text>
            <Text style={styles.infoText}>
              Choose a strong password with at least 8 characters, including uppercase letters and numbers.
            </Text>
          </View>
        </View>

        {/* Current Password */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Current Password</Text>
          
          <View style={styles.inputGroup}>
            <Text style={styles.inputLabel}>Enter Current Password</Text>
            <View style={styles.inputContainer}>
              <Ionicons name="lock-closed-outline" size={20} color="#8A8FB0" />
              <TextInput
                style={styles.input}
                placeholder="Enter your current password"
                placeholderTextColor="#C0C0D0"
                value={currentPassword}
                onChangeText={setCurrentPassword}
                secureTextEntry={!showCurrentPassword}
                autoCapitalize="none"
              />
              <TouchableOpacity onPress={() => setShowCurrentPassword(!showCurrentPassword)}>
                <Ionicons 
                  name={showCurrentPassword ? "eye-off-outline" : "eye-outline"} 
                  size={20} 
                  color="#8A8FB0" 
                />
              </TouchableOpacity>
            </View>
          </View>
        </View>

        {/* New Password */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>New Password</Text>
          
          <View style={styles.inputGroup}>
            <Text style={styles.inputLabel}>Enter New Password</Text>
            <View style={[
              styles.inputContainer,
              newPassword && newPassword.length < 6 && styles.inputError
            ]}>
              <Ionicons name="lock-closed-outline" size={20} color="#8A8FB0" />
              <TextInput
                style={styles.input}
                placeholder="Enter your new password"
                placeholderTextColor="#C0C0D0"
                value={newPassword}
                onChangeText={setNewPassword}
                secureTextEntry={!showNewPassword}
                autoCapitalize="none"
              />
              <TouchableOpacity onPress={() => setShowNewPassword(!showNewPassword)}>
                <Ionicons 
                  name={showNewPassword ? "eye-off-outline" : "eye-outline"} 
                  size={20} 
                  color="#8A8FB0" 
                />
              </TouchableOpacity>
            </View>

            {/* Password Strength Indicator */}
            {newPassword.length > 0 && (
              <View style={styles.strengthContainer}>
                <View style={styles.strengthBars}>
                  {[1, 2, 3].map((level) => (
                    <View
                      key={level}
                      style={[
                        styles.strengthBar,
                        {
                          backgroundColor: level <= passwordStrength.strength
                            ? passwordStrength.color
                            : '#E6E8F0'
                        }
                      ]}
                    />
                  ))}
                </View>
                <Text style={[styles.strengthLabel, { color: passwordStrength.color }]}>
                  {passwordStrength.label}
                </Text>
              </View>
            )}
          </View>

          {/* Confirm New Password */}
          <View style={styles.inputGroup}>
            <Text style={styles.inputLabel}>Confirm New Password</Text>
            <View style={[
              styles.inputContainer,
              confirmPassword && newPassword !== confirmPassword && styles.inputError
            ]}>
              <Ionicons name="lock-closed-outline" size={20} color="#8A8FB0" />
              <TextInput
                style={styles.input}
                placeholder="Re-enter your new password"
                placeholderTextColor="#C0C0D0"
                value={confirmPassword}
                onChangeText={setConfirmPassword}
                secureTextEntry={!showConfirmPassword}
                autoCapitalize="none"
              />
              <TouchableOpacity onPress={() => setShowConfirmPassword(!showConfirmPassword)}>
                <Ionicons 
                  name={showConfirmPassword ? "eye-off-outline" : "eye-outline"} 
                  size={20} 
                  color="#8A8FB0" 
                />
              </TouchableOpacity>
            </View>
            {confirmPassword && newPassword === confirmPassword && (
              <View style={styles.matchIndicator}>
                <Ionicons name="checkmark-circle" size={16} color="#00D4AA" />
                <Text style={styles.matchText}>Passwords match</Text>
              </View>
            )}
            {confirmPassword && newPassword !== confirmPassword && (
              <Text style={styles.errorText}>Passwords do not match</Text>
            )}
          </View>
        </View>

        {/* Password Requirements */}
        <View style={styles.requirementsCard}>
          <Text style={styles.requirementsTitle}>Password Requirements:</Text>
          <View style={styles.requirementItem}>
            <Ionicons 
              name={newPassword.length >= 8 ? "checkmark-circle" : "ellipse-outline"} 
              size={20} 
              color={newPassword.length >= 8 ? "#00D4AA" : "#C0C0D0"} 
            />
            <Text style={styles.requirementText}>At least 8 characters</Text>
          </View>
          <View style={styles.requirementItem}>
            <Ionicons 
              name={/[A-Z]/.test(newPassword) ? "checkmark-circle" : "ellipse-outline"} 
              size={20} 
              color={/[A-Z]/.test(newPassword) ? "#00D4AA" : "#C0C0D0"} 
            />
            <Text style={styles.requirementText}>One uppercase letter</Text>
          </View>
          <View style={styles.requirementItem}>
            <Ionicons 
              name={/[0-9]/.test(newPassword) ? "checkmark-circle" : "ellipse-outline"} 
              size={20} 
              color={/[0-9]/.test(newPassword) ? "#00D4AA" : "#C0C0D0"} 
            />
            <Text style={styles.requirementText}>One number</Text>
          </View>
        </View>

        {/* Change Password Button */}
        <TouchableOpacity 
          style={styles.changeButton}
          onPress={handleChangePassword}
          disabled={isLoading}
          activeOpacity={0.8}
        >
          <LinearGradient
            colors={isLoading ? ['#C0C0D0', '#A0A0B0'] : ['#5F3BFF', '#3B1DFF']}
            start={{ x: 0, y: 0 }}
            end={{ x: 1, y: 0 }}
            style={styles.changeGradient}
          >
            {isLoading ? (
              <Text style={styles.changeButtonText}>Updating...</Text>
            ) : (
              <>
                <Ionicons name="lock-closed" size={20} color="#fff" />
                <Text style={styles.changeButtonText}>Change Password</Text>
              </>
            )}
          </LinearGradient>
        </TouchableOpacity>

        {/* Cancel Button */}
        <TouchableOpacity 
          style={styles.cancelButton}
          onPress={() => navigation.goBack()}
        >
          <Text style={styles.cancelButtonText}>Cancel</Text>
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
              <Ionicons name="shield-checkmark" size={60} color="#fff" />
            </LinearGradient>
            
            <Text style={styles.successTitle}>Password Changed!</Text>
            <Text style={styles.successMessage}>
              Your password has been updated successfully
            </Text>

            <View style={styles.successCheck}>
              <Ionicons name="checkmark" size={24} color="#00D4AA" />
            </View>
          </Animated.View>
        </View>
      </Modal>
    </KeyboardAvoidingView>
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

  /* ================= INFO CARD ================= */
  infoCard: {
    backgroundColor: '#E8E1FF',
    borderRadius: 16,
    padding: 16,
    flexDirection: 'row',
    marginBottom: 24,
    borderWidth: 1,
    borderColor: '#D0C4FF',
  },

  infoIconContainer: {
    marginRight: 12,
  },

  infoTextContainer: {
    flex: 1,
  },

  infoTitle: {
    color: '#5F3BFF',
    fontSize: 15,
    fontWeight: '800',
    marginBottom: 4,
  },

  infoText: {
    color: '#5F3BFF',
    fontSize: 13,
    fontWeight: '600',
    lineHeight: 20,
  },

  /* ================= SECTIONS ================= */
  section: {
    marginBottom: 24,
  },

  sectionTitle: {
    color: '#231c63',
    fontSize: 18,
    fontWeight: '800',
    marginBottom: 16,
  },

  /* ================= INPUTS ================= */
  inputGroup: {
    marginBottom: 20,
  },

  inputLabel: {
    color: '#231c63',
    fontSize: 15,
    fontWeight: '700',
    marginBottom: 8,
  },

  inputContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#FFFFFF',
    borderRadius: 12,
    paddingHorizontal: 16,
    paddingVertical: 4,
    borderWidth: 2,
    borderColor: '#E6E8F0',
  },

  inputError: {
    borderColor: '#FF6B9D',
  },

  input: {
    flex: 1,
    paddingVertical: 12,
    paddingLeft: 12,
    color: '#231c63',
    fontSize: 16,
    fontWeight: '600',
  },

  errorText: {
    color: '#FF6B9D',
    fontSize: 13,
    fontWeight: '600',
    marginTop: 6,
    marginLeft: 4,
  },

  /* ================= PASSWORD STRENGTH ================= */
  strengthContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 8,
  },

  strengthBars: {
    flexDirection: 'row',
    flex: 1,
    gap: 6,
  },

  strengthBar: {
    flex: 1,
    height: 4,
    borderRadius: 2,
  },

  strengthLabel: {
    fontSize: 13,
    fontWeight: '700',
    marginLeft: 12,
  },

  /* ================= MATCH INDICATOR ================= */
  matchIndicator: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 6,
  },

  matchText: {
    color: '#00D4AA',
    fontSize: 13,
    fontWeight: '600',
    marginLeft: 6,
  },

  /* ================= REQUIREMENTS CARD ================= */
  requirementsCard: {
    backgroundColor: '#FFFFFF',
    borderRadius: 16,
    padding: 16,
    marginBottom: 24,
    borderWidth: 1,
    borderColor: '#E6E8F0',
  },

  requirementsTitle: {
    color: '#231c63',
    fontSize: 15,
    fontWeight: '800',
    marginBottom: 12,
  },

  requirementItem: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 8,
  },

  requirementText: {
    color: '#8A8FB0',
    fontSize: 14,
    fontWeight: '600',
    marginLeft: 10,
  },

  /* ================= BUTTONS ================= */
  changeButton: {
    borderRadius: 16,
    overflow: 'hidden',
    shadowColor: '#5F3BFF',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 12,
    elevation: 6,
    marginBottom: 16,
  },

  changeGradient: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 16,
    paddingHorizontal: 24,
  },

  changeButtonText: {
    color: '#FFFFFF',
    fontSize: 16,
    fontWeight: '800',
    marginLeft: 8,
  },

  cancelButton: {
    paddingVertical: 14,
    alignItems: 'center',
  },

  cancelButtonText: {
    color: '#8A8FB0',
    fontSize: 16,
    fontWeight: '700',
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