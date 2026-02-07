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
  Image,
  KeyboardAvoidingView,
  Platform,
  Modal,
  Animated
} from 'react-native'

import { useNavigation } from '@react-navigation/native'
import { Ionicons } from '@expo/vector-icons'
import { LinearGradient } from 'expo-linear-gradient'

export default function EditProfileScreen() {
  const navigation = useNavigation()
  
  const [firstName, setFirstName] = useState('Jenifer')
  const [lastName, setLastName] = useState('Anderson')
  const [phoneNumber, setPhoneNumber] = useState('+1 (555) 123-4567')
  const [isLoading, setIsLoading] = useState(false)
  const [showSuccessModal, setShowSuccessModal] = useState(false)
  const scaleAnim = useState(new Animated.Value(0))[0]

  const handleSaveChanges = () => {
    // Validation
    if (!firstName.trim()) {
      Alert.alert('Error', 'Please enter your first name')
      return
    }

    if (!lastName.trim()) {
      Alert.alert('Error', 'Please enter your last name')
      return
    }

    // Confirm save
    Alert.alert(
      'Save Changes',
      'Are you sure you want to save these changes?',
      [
        {
          text: 'Cancel',
          style: 'cancel'
        },
        {
          text: 'Save',
          onPress: processSave
        }
      ]
    )
  }

  const processSave = async () => {
    setIsLoading(true)

    // Simulate API call
    setTimeout(() => {
      setIsLoading(false)
      showSuccessMessage()

      // Here you would call your API:
      // try {
      //   const response = await updateProfileAPI({ firstName, lastName, phoneNumber })
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

  const handleChangePhoto = () => {
    Alert.alert(
      'Change Profile Photo',
      'Choose an option',
      [
        {
          text: 'Take Photo',
          onPress: () => console.log('Open Camera - EditProfileScreen.js:114')
        },
        {
          text: 'Choose from Gallery',
          onPress: () => console.log('Open Gallery - EditProfileScreen.js:118')
        },
        {
          text: 'Cancel',
          style: 'cancel'
        }
      ]
    )
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
            <Text style={styles.headerTitle}>Edit Profile</Text>
            <Text style={styles.headerSubtitle}>Update your information</Text>
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
        {/* Profile Photo Section */}
        <View style={styles.photoSection}>
          <View style={styles.avatarContainer}>
            <Image 
              source={require('../../../../../assets/images/parent-avatar.png')}
              style={styles.avatar}
              defaultSource={require('../../../../../assets/images/default-avatar.png')}
            />
            <TouchableOpacity 
              style={styles.changePhotoButton}
              onPress={handleChangePhoto}
            >
              <LinearGradient
                colors={['#5F3BFF', '#3B1DFF']}
                start={{ x: 0, y: 0 }}
                end={{ x: 1, y: 1 }}
                style={styles.changePhotoGradient}
              >
                <Ionicons name="camera" size={24} color="#fff" />
              </LinearGradient>
            </TouchableOpacity>
          </View>
          <TouchableOpacity onPress={handleChangePhoto}>
            <Text style={styles.changePhotoText}>Change Photo</Text>
          </TouchableOpacity>
        </View>

        {/* Personal Information */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Personal Information</Text>

          {/* First Name */}
          <View style={styles.inputGroup}>
            <Text style={styles.inputLabel}>First Name</Text>
            <View style={styles.inputContainer}>
              <Ionicons name="person-outline" size={20} color="#8A8FB0" />
              <TextInput
                style={styles.input}
                placeholder="Enter first name"
                placeholderTextColor="#C0C0D0"
                value={firstName}
                onChangeText={setFirstName}
                autoCapitalize="words"
              />
              {firstName.trim() && (
                <Ionicons name="checkmark-circle" size={20} color="#00D4AA" />
              )}
            </View>
          </View>

          {/* Last Name */}
          <View style={styles.inputGroup}>
            <Text style={styles.inputLabel}>Last Name</Text>
            <View style={styles.inputContainer}>
              <Ionicons name="person-outline" size={20} color="#8A8FB0" />
              <TextInput
                style={styles.input}
                placeholder="Enter last name"
                placeholderTextColor="#C0C0D0"
                value={lastName}
                onChangeText={setLastName}
                autoCapitalize="words"
              />
              {lastName.trim() && (
                <Ionicons name="checkmark-circle" size={20} color="#00D4AA" />
              )}
            </View>
          </View>

          {/* Full Name Preview */}
          <View style={styles.previewCard}>
            <Ionicons name="eye" size={18} color="#5F3BFF" />
            <Text style={styles.previewLabel}>Display Name:</Text>
            <Text style={styles.previewValue}>
              {firstName.trim() || lastName.trim() 
                ? `${firstName.trim()} ${lastName.trim()}`.trim()
                : 'Your Name'
              }
            </Text>
          </View>
        </View>

        {/* Contact Information */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Contact Information</Text>

          {/* Phone Number */}
          <View style={styles.inputGroup}>
            <Text style={styles.inputLabel}>Phone Number (Optional)</Text>
            <View style={styles.inputContainer}>
              <Ionicons name="call-outline" size={20} color="#8A8FB0" />
              <TextInput
                style={styles.input}
                placeholder="Enter phone number"
                placeholderTextColor="#C0C0D0"
                value={phoneNumber}
                onChangeText={setPhoneNumber}
                keyboardType="phone-pad"
              />
            </View>
            <Text style={styles.helperText}>
              Used for account recovery and notifications
            </Text>
          </View>

          {/* Email (Read-only) */}
          <View style={styles.inputGroup}>
            <Text style={styles.inputLabel}>Email Address</Text>
            <View style={[styles.inputContainer, styles.readOnlyInput]}>
              <Ionicons name="mail-outline" size={20} color="#8A8FB0" />
              <Text style={styles.readOnlyText}>jenifer.anderson@email.com</Text>
              <View style={styles.verifiedBadge}>
                <Ionicons name="checkmark-circle" size={16} color="#00D4AA" />
              </View>
            </View>
            <TouchableOpacity 
              style={styles.changeEmailLink}
              onPress={() => navigation.navigate('ChangeEmail')}
            >
              <Text style={styles.changeEmailText}>Change Email Address</Text>
              <Ionicons name="arrow-forward" size={16} color="#5F3BFF" />
            </TouchableOpacity>
          </View>
        </View>

        {/* Info Notice */}
        <View style={styles.infoNotice}>
          <Ionicons name="information-circle" size={20} color="#5F3BFF" />
          <Text style={styles.infoText}>
            Your information is encrypted and kept secure. We'll never share it without your permission.
          </Text>
        </View>

        {/* Save Button */}
        <TouchableOpacity 
          style={styles.saveButton}
          onPress={handleSaveChanges}
          disabled={isLoading}
          activeOpacity={0.8}
        >
          <LinearGradient
            colors={isLoading ? ['#C0C0D0', '#A0A0B0'] : ['#5F3BFF', '#3B1DFF']}
            start={{ x: 0, y: 0 }}
            end={{ x: 1, y: 0 }}
            style={styles.saveGradient}
          >
            {isLoading ? (
              <Text style={styles.saveButtonText}>Saving...</Text>
            ) : (
              <>
                <Ionicons name="checkmark-circle" size={20} color="#fff" />
                <Text style={styles.saveButtonText}>Save Changes</Text>
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
              <Ionicons name="checkmark-circle" size={60} color="#fff" />
            </LinearGradient>
            
            <Text style={styles.successTitle}>Success!</Text>
            <Text style={styles.successMessage}>
              Your profile has been updated successfully
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

  /* ================= PHOTO SECTION ================= */
  photoSection: {
    alignItems: 'center',
    marginBottom: 32,
  },

  avatarContainer: {
    position: 'relative',
    marginBottom: 12,
  },

  avatar: {
    width: 120,
    height: 120,
    borderRadius: 60,
    borderWidth: 4,
    borderColor: '#FFFFFF',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.2,
    shadowRadius: 12,
    elevation: 8,
    backgroundColor: '#E0E0E0',
  },

  changePhotoButton: {
    position: 'absolute',
    bottom: 0,
    right: 0,
    borderRadius: 20,
    overflow: 'hidden',
    shadowColor: '#5F3BFF',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.4,
    shadowRadius: 8,
    elevation: 6,
  },

  changePhotoGradient: {
    width: 40,
    height: 40,
    borderRadius: 20,
    justifyContent: 'center',
    alignItems: 'center',
    borderWidth: 3,
    borderColor: '#FFFFFF',
  },

  changePhotoText: {
    color: '#5F3BFF',
    fontSize: 16,
    fontWeight: '700',
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

  input: {
    flex: 1,
    paddingVertical: 12,
    paddingLeft: 12,
    color: '#231c63',
    fontSize: 16,
    fontWeight: '600',
  },

  helperText: {
    color: '#8A8FB0',
    fontSize: 13,
    fontWeight: '500',
    marginTop: 6,
    marginLeft: 4,
  },

  /* ================= READ-ONLY INPUT ================= */
  readOnlyInput: {
    backgroundColor: '#F8F9FC',
    borderColor: '#E6E8F0',
  },

  readOnlyText: {
    flex: 1,
    paddingVertical: 12,
    paddingLeft: 12,
    color: '#8A8FB0',
    fontSize: 16,
    fontWeight: '600',
  },

  verifiedBadge: {
    marginLeft: 8,
  },

  changeEmailLink: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 8,
  },

  changeEmailText: {
    color: '#5F3BFF',
    fontSize: 14,
    fontWeight: '700',
    marginRight: 4,
  },

  /* ================= PREVIEW CARD ================= */
  previewCard: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#F0ECFF',
    padding: 14,
    borderRadius: 12,
    marginTop: 8,
  },

  previewLabel: {
    color: '#5F3BFF',
    fontSize: 14,
    fontWeight: '600',
    marginLeft: 8,
  },

  previewValue: {
    color: '#231c63',
    fontSize: 14,
    fontWeight: '800',
    marginLeft: 8,
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

  /* ================= BUTTONS ================= */
  saveButton: {
    borderRadius: 16,
    overflow: 'hidden',
    shadowColor: '#5F3BFF',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 12,
    elevation: 6,
    marginBottom: 16,
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