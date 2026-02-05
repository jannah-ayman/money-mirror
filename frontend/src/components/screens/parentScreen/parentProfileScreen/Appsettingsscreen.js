import React, { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  Switch,
  StatusBar,
  Modal,
  Animated,
  Alert,
} from 'react-native';
import { useNavigation } from '@react-navigation/native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';

export default function AppSettingsScreen() {
  const navigation = useNavigation();

  // Settings States
  const [darkMode, setDarkMode] = useState(false);
  const [soundEffects, setSoundEffects] = useState(true);
  const [hapticFeedback, setHapticFeedback] = useState(true);
  const [autoSync, setAutoSync] = useState(true);
  const [offlineMode, setOfflineMode] = useState(false);
  const [biometricAuth, setBiometricAuth] = useState(false);
  const [autoLock, setAutoLock] = useState(true);
  const [shareAnalytics, setShareAnalytics] = useState(true);

  // Language & Currency
  const [selectedLanguage, setSelectedLanguage] = useState('English');
  const [selectedCurrency, setSelectedCurrency] = useState('EGP');

  // Modal states
  const [showSuccessModal, setShowSuccessModal] = useState(false);
  const [scaleAnim] = useState(new Animated.Value(0));
  const [showLanguageModal, setShowLanguageModal] = useState(false);
  const [showCurrencyModal, setShowCurrencyModal] = useState(false);

  const languages = ['English', 'العربية', 'Français', 'Español'];
  const currencies = ['EGP', 'USD', 'EUR', 'GBP', 'SAR', 'AED'];

  const handleSaveSettings = () => {
    setShowSuccessModal(true);
    
    Animated.spring(scaleAnim, {
      toValue: 1,
      tension: 50,
      friction: 7,
      useNativeDriver: true,
    }).start();

    setTimeout(() => {
      Animated.timing(scaleAnim, {
        toValue: 0,
        duration: 200,
        useNativeDriver: true,
      }).start(() => {
        setShowSuccessModal(false);
      });
    }, 1500);
  };

  const handleClearCache = () => {
    Alert.alert(
      'Clear Cache',
      'Are you sure you want to clear the app cache? This will free up storage space.',
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Clear',
          style: 'destructive',
          onPress: () => {
            Alert.alert('Success', 'Cache cleared successfully!');
          },
        },
      ]
    );
  };

  const handleResetSettings = () => {
    Alert.alert(
      'Reset All Settings',
      'This will restore all settings to their default values. Are you sure?',
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Reset',
          style: 'destructive',
          onPress: () => {
            // Reset all settings
            setDarkMode(false);
            setSoundEffects(true);
            setHapticFeedback(true);
            setAutoSync(true);
            setOfflineMode(false);
            setBiometricAuth(false);
            setAutoLock(true);
            setShareAnalytics(true);
            Alert.alert('Success', 'Settings reset to default!');
          },
        },
      ]
    );
  };

  const renderSettingItem = (icon, title, description, value, onValueChange, color = '#5F3BFF') => (
    <View style={styles.settingItem}>
      <View style={styles.settingLeft}>
        <View style={[styles.settingIconContainer, { backgroundColor: color + '20' }]}>
          <Ionicons name={icon} size={22} color={color} />
        </View>
        <View style={styles.settingTextContainer}>
          <Text style={styles.settingTitle}>{title}</Text>
          {description && <Text style={styles.settingDescription}>{description}</Text>}
        </View>
      </View>
      <Switch
        value={value}
        onValueChange={onValueChange}
        trackColor={{ false: '#D1D1D1', true: color + '80' }}
        thumbColor={value ? color : '#f4f3f4'}
        ios_backgroundColor="#D1D1D1"
      />
    </View>
  );

  const renderSelectItem = (icon, title, value, onPress, color = '#5F3BFF') => (
    <TouchableOpacity style={styles.selectItem} onPress={onPress}>
      <View style={styles.settingLeft}>
        <View style={[styles.settingIconContainer, { backgroundColor: color + '20' }]}>
          <Ionicons name={icon} size={22} color={color} />
        </View>
        <View style={styles.settingTextContainer}>
          <Text style={styles.settingTitle}>{title}</Text>
          <Text style={styles.selectedValue}>{value}</Text>
        </View>
      </View>
      <Ionicons name="chevron-forward" size={20} color="#999" />
    </TouchableOpacity>
  );

  const renderActionItem = (icon, title, onPress, color = '#5F3BFF', destructive = false) => (
    <TouchableOpacity 
      style={styles.actionItem} 
      onPress={onPress}
      activeOpacity={0.7}
    >
      <View style={[styles.settingIconContainer, { backgroundColor: color + '20' }]}>
        <Ionicons name={icon} size={22} color={color} />
      </View>
      <Text style={[styles.actionTitle, destructive && { color: '#FF3B30' }]}>{title}</Text>
      <Ionicons name="chevron-forward" size={20} color="#999" />
    </TouchableOpacity>
  );

  return (
    <View style={styles.container}>
      <StatusBar barStyle="light-content" backgroundColor="#1F1147" />

      {/* Header */}
      <LinearGradient
        colors={['#1F1147', '#5F3BFF']}
        start={{ x: 0, y: 0 }}
        end={{ x: 1, y: 1 }}
        style={styles.header}
      >
        <TouchableOpacity
          style={styles.backButton}
          onPress={() => navigation.goBack()}
        >
          <Ionicons name="arrow-back" size={24} color="#fff" />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>App Settings</Text>
        <View style={styles.placeholder} />
      </LinearGradient>

      <ScrollView
        style={styles.scrollView}
        contentContainerStyle={styles.scrollContent}
        showsVerticalScrollIndicator={false}
      >
        {/* Appearance Section */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Appearance</Text>
          <View style={styles.sectionCard}>
            {renderSettingItem(
              'moon',
              'Dark Mode',
              'Switch to dark theme',
              darkMode,
              setDarkMode,
              '#5F3BFF'
            )}
            <View style={styles.divider} />
            {renderSettingItem(
              'volume-high',
              'Sound Effects',
              'Play sounds for actions',
              soundEffects,
              setSoundEffects,
              '#FF6B9D'
            )}
            <View style={styles.divider} />
            {renderSettingItem(
              'phone-portrait',
              'Haptic Feedback',
              'Vibrate on interactions',
              hapticFeedback,
              setHapticFeedback,
              '#00D4AA'
            )}
          </View>
        </View>

        {/* Language & Currency */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Localization</Text>
          <View style={styles.sectionCard}>
            {renderSelectItem(
              'language',
              'Language',
              selectedLanguage,
              () => setShowLanguageModal(true),
              '#5F3BFF'
            )}
            <View style={styles.divider} />
            {renderSelectItem(
              'cash',
              'Currency',
              selectedCurrency,
              () => setShowCurrencyModal(true),
              '#FFB800'
            )}
          </View>
        </View>

        {/* Data & Sync Section */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Data & Sync</Text>
          <View style={styles.sectionCard}>
            {renderSettingItem(
              'sync',
              'Auto Sync',
              'Sync data automatically',
              autoSync,
              setAutoSync,
              '#00D4AA'
            )}
            <View style={styles.divider} />
            {renderSettingItem(
              'cloud-offline',
              'Offline Mode',
              'Work without internet',
              offlineMode,
              setOfflineMode,
              '#999'
            )}
          </View>
        </View>

        {/* Security Section */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Security</Text>
          <View style={styles.sectionCard}>
            {renderSettingItem(
              'finger-print',
              'Biometric Authentication',
              'Use fingerprint or face ID',
              biometricAuth,
              setBiometricAuth,
              '#FF6B9D'
            )}
            <View style={styles.divider} />
            {renderSettingItem(
              'lock-closed',
              'Auto Lock',
              'Lock app when inactive',
              autoLock,
              setAutoLock,
              '#FF3B30'
            )}
          </View>
        </View>

        {/* Privacy Section */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Privacy</Text>
          <View style={styles.sectionCard}>
            {renderSettingItem(
              'analytics',
              'Share Analytics',
              'Help improve the app',
              shareAnalytics,
              setShareAnalytics,
              '#00D4AA'
            )}
          </View>
        </View>

        {/* Storage Section */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Storage</Text>
          <View style={styles.sectionCard}>
            {renderActionItem(
              'trash',
              'Clear Cache',
              handleClearCache,
              '#FFB800'
            )}
            <View style={styles.divider} />
            {renderActionItem(
              'refresh',
              'Reset All Settings',
              handleResetSettings,
              '#FF3B30',
              true
            )}
          </View>
        </View>

        {/* App Info Section */}
        <View style={styles.appInfoSection}>
          <View style={styles.appInfoRow}>
            <Text style={styles.appInfoLabel}>Version</Text>
            <Text style={styles.appInfoValue}>1.0.0</Text>
          </View>
          <View style={styles.appInfoRow}>
            <Text style={styles.appInfoLabel}>Build</Text>
            <Text style={styles.appInfoValue}>2026.02.05</Text>
          </View>
        </View>

        {/* Save Button */}
        <TouchableOpacity
          style={styles.saveButtonContainer}
          onPress={handleSaveSettings}
          activeOpacity={0.8}
        >
          <LinearGradient
            colors={['#5F3BFF', '#1F1147']}
            start={{ x: 0, y: 0 }}
            end={{ x: 1, y: 1 }}
            style={styles.saveButton}
          >
            <Ionicons name="checkmark-circle" size={24} color="#fff" />
            <Text style={styles.saveButtonText}>Save Settings</Text>
          </LinearGradient>
        </TouchableOpacity>

        <View style={styles.bottomPadding} />
      </ScrollView>

      {/* Language Selection Modal */}
      <Modal
        visible={showLanguageModal}
        transparent
        animationType="slide"
      >
        <View style={styles.modalOverlay}>
          <View style={styles.modalContainer}>
            <View style={styles.modalHeader}>
              <Text style={styles.modalTitle}>Select Language</Text>
              <TouchableOpacity onPress={() => setShowLanguageModal(false)}>
                <Ionicons name="close" size={24} color="#666" />
              </TouchableOpacity>
            </View>
            <ScrollView>
              {languages.map((lang) => (
                <TouchableOpacity
                  key={lang}
                  style={styles.modalItem}
                  onPress={() => {
                    setSelectedLanguage(lang);
                    setShowLanguageModal(false);
                  }}
                >
                  <Text style={styles.modalItemText}>{lang}</Text>
                  {selectedLanguage === lang && (
                    <Ionicons name="checkmark" size={24} color="#5F3BFF" />
                  )}
                </TouchableOpacity>
              ))}
            </ScrollView>
          </View>
        </View>
      </Modal>

      {/* Currency Selection Modal */}
      <Modal
        visible={showCurrencyModal}
        transparent
        animationType="slide"
      >
        <View style={styles.modalOverlay}>
          <View style={styles.modalContainer}>
            <View style={styles.modalHeader}>
              <Text style={styles.modalTitle}>Select Currency</Text>
              <TouchableOpacity onPress={() => setShowCurrencyModal(false)}>
                <Ionicons name="close" size={24} color="#666" />
              </TouchableOpacity>
            </View>
            <ScrollView>
              {currencies.map((currency) => (
                <TouchableOpacity
                  key={currency}
                  style={styles.modalItem}
                  onPress={() => {
                    setSelectedCurrency(currency);
                    setShowCurrencyModal(false);
                  }}
                >
                  <Text style={styles.modalItemText}>{currency}</Text>
                  {selectedCurrency === currency && (
                    <Ionicons name="checkmark" size={24} color="#5F3BFF" />
                  )}
                </TouchableOpacity>
              ))}
            </ScrollView>
          </View>
        </View>
      </Modal>

      {/* Success Modal */}
      <Modal
        visible={showSuccessModal}
        transparent
        animationType="fade"
      >
        <View style={styles.successModalOverlay}>
          <Animated.View
            style={[
              styles.successModalContent,
              {
                transform: [{ scale: scaleAnim }],
              },
            ]}
          >
            <LinearGradient
              colors={['#00D4AA', '#00B894']}
              style={styles.successIconContainer}
            >
              <Ionicons name="checkmark" size={40} color="#fff" />
            </LinearGradient>
            <Text style={styles.successTitle}>Saved!</Text>
            <Text style={styles.successMessage}>
              Your settings have been saved successfully.
            </Text>
          </Animated.View>
        </View>
      </Modal>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#F5F7FA',
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingTop: 50,
    paddingBottom: 20,
    paddingHorizontal: 20,
  },
  backButton: {
    padding: 8,
  },
  headerTitle: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#fff',
  },
  placeholder: {
    width: 40,
  },
  scrollView: {
    flex: 1,
  },
  scrollContent: {
    paddingBottom: 30,
  },
  section: {
    marginTop: 20,
    paddingHorizontal: 20,
  },
  sectionTitle: {
    fontSize: 14,
    fontWeight: '600',
    color: '#666',
    marginBottom: 10,
    textTransform: 'uppercase',
    letterSpacing: 0.5,
  },
  sectionCard: {
    backgroundColor: '#fff',
    borderRadius: 12,
    paddingHorizontal: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },
  settingItem: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingVertical: 16,
  },
  settingLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },
  settingIconContainer: {
    width: 44,
    height: 44,
    borderRadius: 22,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 12,
  },
  settingTextContainer: {
    flex: 1,
  },
  settingTitle: {
    fontSize: 15,
    fontWeight: '500',
    color: '#1F1147',
    marginBottom: 2,
  },
  settingDescription: {
    fontSize: 13,
    color: '#999',
  },
  divider: {
    height: 1,
    backgroundColor: '#F0F0F0',
    marginLeft: 56,
  },
  selectItem: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingVertical: 16,
  },
  selectedValue: {
    fontSize: 13,
    color: '#5F3BFF',
    fontWeight: '500',
    marginTop: 2,
  },
  actionItem: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingVertical: 16,
  },
  actionTitle: {
    flex: 1,
    fontSize: 15,
    fontWeight: '500',
    color: '#1F1147',
    marginLeft: 12,
  },
  appInfoSection: {
    marginTop: 20,
    marginHorizontal: 20,
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },
  appInfoRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingVertical: 8,
  },
  appInfoLabel: {
    fontSize: 14,
    color: '#666',
  },
  appInfoValue: {
    fontSize: 14,
    fontWeight: '500',
    color: '#1F1147',
  },
  saveButtonContainer: {
    marginHorizontal: 20,
    marginTop: 30,
    borderRadius: 12,
    overflow: 'hidden',
    shadowColor: '#5F3BFF',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 5,
    marginBottom: 40,
  },
  saveButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 16,
  },
  saveButtonText: {
    fontSize: 16,
    fontWeight: '600',
    color: '#fff',
    marginLeft: 8,
  },
  bottomPadding: {
    height: 20,

  },
  // Modal Styles
  modalOverlay: {
    flex: 1,
    backgroundColor: 'rgba(0, 0, 0, 0.5)',
    justifyContent: 'flex-end',
  },
  modalContainer: {
    backgroundColor: '#fff',
    borderTopLeftRadius: 20,
    borderTopRightRadius: 20,
    paddingBottom: 30,
    maxHeight: '70%',
  },
  modalHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: 20,
    borderBottomWidth: 1,
    borderBottomColor: '#F0F0F0',
  },
  modalTitle: {
    fontSize: 18,
    fontWeight: '600',
    color: '#1F1147',
  },
  modalItem: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: 16,
    paddingHorizontal: 20,
    borderBottomWidth: 1,
    borderBottomColor: '#F5F5F5',
  },
  modalItemText: {
    fontSize: 16,
    color: '#333',
  },
  // Success Modal
  successModalOverlay: {
    flex: 1,
    backgroundColor: 'rgba(0, 0, 0, 0.5)',
    justifyContent: 'center',
    alignItems: 'center',
  },
  successModalContent: {
    backgroundColor: '#fff',
    borderRadius: 20,
    padding: 30,
    alignItems: 'center',
    width: '80%',
    maxWidth: 320,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 10 },
    shadowOpacity: 0.25,
    shadowRadius: 20,
    elevation: 10,
  },
  successIconContainer: {
    width: 80,
    height: 80,
    borderRadius: 40,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 20,
  },
  successTitle: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#1F1147',
    marginBottom: 10,
  },
  successMessage: {
    fontSize: 15,
    color: '#666',
    textAlign: 'center',
    lineHeight: 22,
  },
});