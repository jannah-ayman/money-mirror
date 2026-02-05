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
  Platform
} from 'react-native'

import { useNavigation } from '@react-navigation/native'
import { Ionicons } from '@expo/vector-icons'
import { LinearGradient } from 'expo-linear-gradient'

export default function ChangeEmailScreen() {
  const navigation = useNavigation()
  
  const [currentEmail, setCurrentEmail] = useState('jenifer.anderson@email.com')
  const [newEmail, setNewEmail] = useState('')
  const [confirmEmail, setConfirmEmail] = useState('')
  const [password, setPassword] = useState('')
  
  const [showPassword, setShowPassword] = useState(false)
  const [isLoading, setIsLoading] = useState(false)

  // Email validation
  const isValidEmail = (email) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    return emailRegex.test(email)
  }

  const handleChangeEmail = () => {
    // Validation
    if (!newEmail.trim()) {
      Alert.alert('Error', 'Please enter a new email address')
      return
    }

    if (!isValidEmail(newEmail)) {
      Alert.alert('Error', 'Please enter a valid email address')
      return
    }

    if (newEmail === currentEmail) {
      Alert.alert('Error', 'New email must be different from current email')
      return
    }

    if (newEmail !== confirmEmail) {
      Alert.alert('Error', 'Email addresses do not match')
      return
    }

    if (!password.trim()) {
      Alert.alert('Error', 'Please enter your password to confirm')
      return
    }

    // Show confirmation dialog
    Alert.alert(
      'Confirm Email Change',
      `Are you sure you want to change your email to:\n\n${newEmail}\n\nA verification link will be sent to your new email address.`,
      [
        {
          text: 'Cancel',
          style: 'cancel'
        },
        {
          text: 'Confirm',
          onPress: processEmailChange
        }
      ]
    )
  }

  const processEmailChange = async () => {
    setIsLoading(true)

    // Simulate API call
    setTimeout(() => {
      setIsLoading(false)
      
      Alert.alert(
        'Verification Email Sent! ✉️',
        `A verification link has been sent to ${newEmail}.\n\nPlease check your inbox and click the link to verify your new email address.`,
        [
          {
            text: 'OK',
            onPress: () => navigation.goBack()
          }
        ]
      )

      // Here you would call your API:
      // try {
      //   const response = await changeEmailAPI(newEmail, password)
      //   if (response.success) {
      //     Alert.alert('Success', 'Verification email sent!')
      //     navigation.goBack()
      //   }
      // } catch (error) {
      //   Alert.alert('Error', error.message)
      // }
    }, 1500)
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
            <Text style={styles.headerTitle}>Change Email</Text>
            <Text style={styles.headerSubtitle}>Update your email address</Text>
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
        {/* Info Card */}
        <View style={styles.infoCard}>
          <View style={styles.infoIconContainer}>
            <Ionicons name="information-circle" size={24} color="#5F3BFF" />
          </View>
          <View style={styles.infoTextContainer}>
            <Text style={styles.infoTitle}>Important</Text>
            <Text style={styles.infoText}>
              You'll receive a verification email at your new address. Please verify it to complete the change.
            </Text>
          </View>
        </View>

        {/* Current Email */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Current Email</Text>
          <View style={styles.currentEmailCard}>
            <Ionicons name="mail" size={20} color="#8A8FB0" />
            <Text style={styles.currentEmailText}>{currentEmail}</Text>
            <View style={styles.verifiedBadge}>
              <Ionicons name="checkmark-circle" size={16} color="#00D4AA" />
              <Text style={styles.verifiedText}>Verified</Text>
            </View>
          </View>
        </View>

        {/* New Email Input */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>New Email Address</Text>
          <View style={[
            styles.inputContainer,
            newEmail && !isValidEmail(newEmail) && styles.inputError
          ]}>
            <Ionicons name="mail-outline" size={20} color="#8A8FB0" />
            <TextInput
              style={styles.input}
              placeholder="Enter new email address"
              placeholderTextColor="#C0C0D0"
              value={newEmail}
              onChangeText={setNewEmail}
              keyboardType="email-address"
              autoCapitalize="none"
              autoCorrect={false}
            />
            {newEmail && isValidEmail(newEmail) && (
              <Ionicons name="checkmark-circle" size={20} color="#00D4AA" />
            )}
          </View>
          {newEmail && !isValidEmail(newEmail) && (
            <Text style={styles.errorText}>Please enter a valid email address</Text>
          )}
        </View>

        {/* Confirm Email Input */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Confirm New Email</Text>
          <View style={[
            styles.inputContainer,
            confirmEmail && newEmail !== confirmEmail && styles.inputError
          ]}>
            <Ionicons name="mail-outline" size={20} color="#8A8FB0" />
            <TextInput
              style={styles.input}
              placeholder="Confirm new email address"
              placeholderTextColor="#C0C0D0"
              value={confirmEmail}
              onChangeText={setConfirmEmail}
              keyboardType="email-address"
              autoCapitalize="none"
              autoCorrect={false}
            />
            {confirmEmail && newEmail === confirmEmail && (
              <Ionicons name="checkmark-circle" size={20} color="#00D4AA" />
            )}
          </View>
          {confirmEmail && newEmail !== confirmEmail && (
            <Text style={styles.errorText}>Email addresses do not match</Text>
          )}
        </View>

        {/* Password Confirmation */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Confirm Password</Text>
          <View style={styles.inputContainer}>
            <Ionicons name="lock-closed-outline" size={20} color="#8A8FB0" />
            <TextInput
              style={styles.input}
              placeholder="Enter your password"
              placeholderTextColor="#C0C0D0"
              value={password}
              onChangeText={setPassword}
              secureTextEntry={!showPassword}
              autoCapitalize="none"
            />
            <TouchableOpacity onPress={() => setShowPassword(!showPassword)}>
              <Ionicons 
                name={showPassword ? "eye-off-outline" : "eye-outline"} 
                size={20} 
                color="#8A8FB0" 
              />
            </TouchableOpacity>
          </View>
          <Text style={styles.helperText}>
            Enter your current password to verify this change
          </Text>
        </View>

        {/* Security Notice */}
        <View style={styles.securityNotice}>
          <Ionicons name="shield-checkmark" size={20} color="#5F3BFF" />
          <Text style={styles.securityText}>
            Your email is protected with encryption and will never be shared
          </Text>
        </View>

        {/* Change Email Button */}
        <TouchableOpacity 
          style={styles.changeButton}
          onPress={handleChangeEmail}
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
              <Text style={styles.changeButtonText}>Processing...</Text>
            ) : (
              <>
                <Ionicons name="mail" size={20} color="#fff" />
                <Text style={styles.changeButtonText}>Change Email Address</Text>
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
    fontSize: 15,
    fontWeight: '800',
    marginBottom: 12,
  },

  /* ================= CURRENT EMAIL ================= */
  currentEmailCard: {
    backgroundColor: '#FFFFFF',
    borderRadius: 12,
    padding: 16,
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderColor: '#E6E8F0',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },

  currentEmailText: {
    flex: 1,
    color: '#231c63',
    fontSize: 15,
    fontWeight: '600',
    marginLeft: 12,
  },

  verifiedBadge: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#E8F9F6',
    paddingHorizontal: 10,
    paddingVertical: 6,
    borderRadius: 12,
  },

  verifiedText: {
    color: '#00D4AA',
    fontSize: 12,
    fontWeight: '700',
    marginLeft: 4,
  },

  /* ================= INPUTS ================= */
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

  helperText: {
    color: '#8A8FB0',
    fontSize: 13,
    fontWeight: '500',
    marginTop: 6,
    marginLeft: 4,
  },

  /* ================= SECURITY NOTICE ================= */
  securityNotice: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#F0ECFF',
    padding: 14,
    borderRadius: 12,
    marginBottom: 24,
  },

  securityText: {
    flex: 1,
    marginLeft: 12,
    color: '#5F3BFF',
    fontSize: 13,
    fontWeight: '600',
    lineHeight: 18,
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
});