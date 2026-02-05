import React, { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  StatusBar,
  Linking,
  Alert,
} from 'react-native';
import { useNavigation } from '@react-navigation/native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';

export default function HelpSupportScreen() {
  const navigation = useNavigation();
  const [expandedFAQ, setExpandedFAQ] = useState(null);

  const faqData = [
    {
      id: 1,
      question: 'How do I add a new child account?',
      answer: 'Go to "Manage Children" from your profile, tap the "+" button, enter your child\'s information, and save. Each child will receive a unique login code.',
    },
    {
      id: 2,
      question: 'How does the allowance system work?',
      answer: 'You can set up recurring allowances for your children. Go to "Allowance" section, select the child, set the amount and frequency (weekly/monthly), and the system will automatically track it.',
    },
    {
      id: 3,
      question: 'Can I track my child\'s expenses?',
      answer: 'Yes! Your child logs their expenses in their app, and you can view all transactions in real-time through the Reports section.',
    },
    {
      id: 4,
      question: 'How do I set up goals for my child?',
      answer: 'Navigate to "Goals & Challenges", create a new goal with a target amount and deadline. Your child can track their progress and you can monitor it from your dashboard.',
    },
    {
      id: 5,
      question: 'What if I forget my password?',
      answer: 'On the login screen, tap "Forgot Password", enter your email, and follow the reset instructions sent to your inbox.',
    },
    {
      id: 6,
      question: 'Is my financial data secure?',
      answer: 'Absolutely! We use bank-level encryption to protect your data. All information is securely stored and never shared with third parties.',
    },
    {
      id: 7,
      question: 'How do I delete a child account?',
      answer: 'Go to "Manage Children", select the child, tap the remove button. Note: This action will delete all associated data permanently.',
    },
    {
      id: 8,
      question: 'Can multiple parents access the same account?',
      answer: 'Currently, each account is individual. We\'re working on a family sharing feature for future updates!',
    },
  ];

  const contactOptions = [
    {
      id: 1,
      icon: 'mail',
      title: 'Email Support',
      subtitle: 'support@moneymirror.com',
      color: '#5F3BFF',
      action: () => handleEmailSupport(),
    },
    {
      id: 2,
      icon: 'call',
      title: 'Phone Support',
      subtitle: '+20 123 456 7890',
      color: '#00D4AA',
      action: () => handlePhoneSupport(),
    },
    {
      id: 3,
      icon: 'chatbubbles',
      title: 'Live Chat',
      subtitle: 'Chat with our team',
      color: '#FF6B9D',
      action: () => handleLiveChat(),
    },
    {
      id: 4,
      icon: 'logo-whatsapp',
      title: 'WhatsApp',
      subtitle: 'Message us on WhatsApp',
      color: '#25D366',
      action: () => handleWhatsApp(),
    },
  ];

  const resourceLinks = [
    {
      id: 1,
      icon: 'book',
      title: 'User Guide',
      subtitle: 'Complete app documentation',
      color: '#5F3BFF',
      action: () => handleUserGuide(),
    },
    {
      id: 2,
      icon: 'videocam',
      title: 'Video Tutorials',
      subtitle: 'Watch how-to videos',
      color: '#FF6B9D',
      action: () => handleVideoTutorials(),
    },
    {
      id: 3,
      icon: 'bug',
      title: 'Report a Bug',
      subtitle: 'Help us improve',
      color: '#FFB800',
      action: () => handleReportBug(),
    },
    {
      id: 4,
      icon: 'star',
      title: 'Rate Our App',
      subtitle: 'Share your feedback',
      color: '#00D4AA',
      action: () => handleRateApp(),
    },
  ];

  const handleEmailSupport = () => {
    const email = 'support@moneymirror.com';
    const subject = 'Support Request';
    const body = 'Hello Money Mirror Support Team,\n\n';
    
    Linking.openURL(`mailto:${email}?subject=${subject}&body=${body}`).catch(() =>
      Alert.alert('Error', 'Could not open email app')
    );
  };

  const handlePhoneSupport = () => {
    const phoneNumber = '+201234567890';
    Linking.openURL(`tel:${phoneNumber}`).catch(() =>
      Alert.alert('Error', 'Could not open phone app')
    );
  };

  const handleLiveChat = () => {
    Alert.alert(
      'Live Chat',
      'Live chat is available Monday-Friday, 9 AM - 6 PM. Would you like to start a conversation?',
      [
        { text: 'Cancel', style: 'cancel' },
        { text: 'Start Chat - Helpsupportscreen.js:156', onPress: () => console.log('Opening live chat...') },
      ]
    );
  };

  const handleWhatsApp = () => {
    const phoneNumber = '201234567890';
    const message = 'Hello, I need help with Money Mirror app.';
    
    Linking.openURL(`whatsapp://send?phone=${phoneNumber}&text=${message}`).catch(() =>
      Alert.alert('Error', 'WhatsApp is not installed on your device')
    );
  };

  const handleUserGuide = () => {
    Alert.alert('User Guide', 'Opening comprehensive user guide...');
  };

  const handleVideoTutorials = () => {
    Alert.alert('Video Tutorials', 'Opening video tutorial library...');
  };

  const handleReportBug = () => {
    Alert.alert(
      'Report a Bug',
      'Please describe the issue you\'re experiencing. We\'ll investigate and fix it as soon as possible.',
      [
        { text: 'Cancel', style: 'cancel' },
        { text: 'Report - Helpsupportscreen.js:184', onPress: () => console.log('Bug report submitted') },
      ]
    );
  };

  const handleRateApp = () => {
    Alert.alert(
      'Rate Money Mirror',
      'Thank you for using our app! Please rate us on the App Store.',
      [
        { text: 'Later', style: 'cancel' },
        { text: 'Rate Now - Helpsupportscreen.js:195', onPress: () => console.log('Opening app store...') },
      ]
    );
  };

  const toggleFAQ = (id) => {
    setExpandedFAQ(expandedFAQ === id ? null : id);
  };

  const renderContactOption = (item) => (
    <TouchableOpacity
      key={item.id}
      style={styles.contactCard}
      onPress={item.action}
      activeOpacity={0.7}
    >
      <View style={[styles.contactIconContainer, { backgroundColor: item.color + '20' }]}>
        <Ionicons name={item.icon} size={28} color={item.color} />
      </View>
      <View style={styles.contactTextContainer}>
        <Text style={styles.contactTitle}>{item.title}</Text>
        <Text style={styles.contactSubtitle}>{item.subtitle}</Text>
      </View>
      <Ionicons name="chevron-forward" size={20} color="#999" />
    </TouchableOpacity>
  );

  const renderResourceLink = (item) => (
    <TouchableOpacity
      key={item.id}
      style={styles.resourceCard}
      onPress={item.action}
      activeOpacity={0.7}
    >
      <View style={[styles.resourceIconContainer, { backgroundColor: item.color + '20' }]}>
        <Ionicons name={item.icon} size={24} color={item.color} />
      </View>
      <View style={styles.resourceTextContainer}>
        <Text style={styles.resourceTitle}>{item.title}</Text>
        <Text style={styles.resourceSubtitle}>{item.subtitle}</Text>
      </View>
      <Ionicons name="chevron-forward" size={20} color="#999" />
    </TouchableOpacity>
  );

  const renderFAQItem = (item) => (
    <View key={item.id} style={styles.faqCard}>
      <TouchableOpacity
        style={styles.faqHeader}
        onPress={() => toggleFAQ(item.id)}
        activeOpacity={0.7}
      >
        <View style={styles.faqQuestionContainer}>
          <View style={styles.faqIconContainer}>
            <Ionicons name="help-circle" size={20} color="#5F3BFF" />
          </View>
          <Text style={styles.faqQuestion}>{item.question}</Text>
        </View>
        <Ionicons
          name={expandedFAQ === item.id ? 'chevron-up' : 'chevron-down'}
          size={20}
          color="#999"
        />
      </TouchableOpacity>
      {expandedFAQ === item.id && (
        <View style={styles.faqAnswer}>
          <Text style={styles.faqAnswerText}>{item.answer}</Text>
        </View>
      )}
    </View>
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
        <Text style={styles.headerTitle}>Help & Support</Text>
        <View style={styles.placeholder} />
      </LinearGradient>

      <ScrollView
        style={styles.scrollView}
        contentContainerStyle={styles.scrollContent}
        showsVerticalScrollIndicator={false}
      >
        {/* Welcome Card */}
        <LinearGradient
          colors={['#5F3BFF', '#7B5FFF']}
          start={{ x: 0, y: 0 }}
          end={{ x: 1, y: 1 }}
          style={styles.welcomeCard}
        >
          <Ionicons name="headset" size={50} color="#fff" />
          <Text style={styles.welcomeTitle}>How can we help you?</Text>
          <Text style={styles.welcomeText}>
            We're here to assist you 24/7. Choose how you'd like to get support.
          </Text>
        </LinearGradient>

        {/* Contact Options Section */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Contact Us</Text>
          <View style={styles.sectionCard}>
            {contactOptions.map((item) => renderContactOption(item))}
          </View>
        </View>

        {/* FAQ Section */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Frequently Asked Questions</Text>
          <Text style={styles.sectionSubtitle}>
            Quick answers to common questions
          </Text>
          {faqData.map((item) => renderFAQItem(item))}
        </View>

        {/* Resources Section */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Helpful Resources</Text>
          <View style={styles.sectionCard}>
            {resourceLinks.map((item) => renderResourceLink(item))}
          </View>
        </View>

        {/* Emergency Support */}
        <View style={styles.emergencyCard}>
          <View style={styles.emergencyIconContainer}>
            <Ionicons name="warning" size={24} color="#FF3B30" />
          </View>
          <View style={styles.emergencyTextContainer}>
            <Text style={styles.emergencyTitle}>Emergency Support</Text>
            <Text style={styles.emergencyText}>
              For urgent issues, call us directly at{' '}
              <Text style={styles.emergencyPhone}>+20 123 456 7890</Text>
            </Text>
          </View>
        </View>

        {/* App Info */}
        <View style={styles.appInfoCard}>
          <Text style={styles.appInfoTitle}>Money Mirror</Text>
          <Text style={styles.appInfoVersion}>Version 1.0.0</Text>
          <Text style={styles.appInfoCopyright}>
            © 2026 Money Mirror. All rights reserved.
          </Text>
          <View style={styles.socialLinks}>
            <TouchableOpacity style={styles.socialButton}>
              <Ionicons name="logo-facebook" size={24} color="#1877F2" />
            </TouchableOpacity>
            <TouchableOpacity style={styles.socialButton}>
              <Ionicons name="logo-twitter" size={24} color="#1DA1F2" />
            </TouchableOpacity>
            <TouchableOpacity style={styles.socialButton}>
              <Ionicons name="logo-instagram" size={24} color="#E4405F" />
            </TouchableOpacity>
            <TouchableOpacity style={styles.socialButton}>
              <Ionicons name="logo-linkedin" size={24} color="#0A66C2" />
            </TouchableOpacity>
          </View>
        </View>

        <View style={styles.bottomPadding} />
      </ScrollView>
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
  welcomeCard: {
    marginHorizontal: 20,
    marginTop: 20,
    padding: 30,
    borderRadius: 16,
    alignItems: 'center',
    shadowColor: '#5F3BFF',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 5,
  },
  welcomeTitle: {
    fontSize: 22,
    fontWeight: 'bold',
    color: '#fff',
    marginTop: 15,
    marginBottom: 10,
  },
  welcomeText: {
    fontSize: 14,
    color: '#fff',
    textAlign: 'center',
    opacity: 0.9,
    lineHeight: 20,
  },
  section: {
    marginTop: 25,
    paddingHorizontal: 20,
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: '600',
    color: '#1F1147',
    marginBottom: 5,
  },
  sectionSubtitle: {
    fontSize: 13,
    color: '#999',
    marginBottom: 15,
  },
  sectionCard: {
    backgroundColor: '#fff',
    borderRadius: 12,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },
  contactCard: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: 16,
    borderBottomWidth: 1,
    borderBottomColor: '#F5F5F5',
  },
  contactIconContainer: {
    width: 56,
    height: 56,
    borderRadius: 28,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 15,
  },
  contactTextContainer: {
    flex: 1,
  },
  contactTitle: {
    fontSize: 16,
    fontWeight: '600',
    color: '#1F1147',
    marginBottom: 4,
  },
  contactSubtitle: {
    fontSize: 13,
    color: '#666',
  },
  faqCard: {
    backgroundColor: '#fff',
    borderRadius: 12,
    marginBottom: 12,
    overflow: 'hidden',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },
  faqHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: 16,
  },
  faqQuestionContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
    marginRight: 10,
  },
  faqIconContainer: {
    marginRight: 12,
  },
  faqQuestion: {
    fontSize: 15,
    fontWeight: '500',
    color: '#1F1147',
    flex: 1,
  },
  faqAnswer: {
    paddingHorizontal: 16,
    paddingBottom: 16,
    paddingLeft: 48,
  },
  faqAnswerText: {
    fontSize: 14,
    color: '#666',
    lineHeight: 22,
  },
  resourceCard: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: 16,
    borderBottomWidth: 1,
    borderBottomColor: '#F5F5F5',
  },
  resourceIconContainer: {
    width: 50,
    height: 50,
    borderRadius: 25,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 15,
  },
  resourceTextContainer: {
    flex: 1,
  },
  resourceTitle: {
    fontSize: 15,
    fontWeight: '600',
    color: '#1F1147',
    marginBottom: 4,
  },
  resourceSubtitle: {
    fontSize: 13,
    color: '#666',
  },
  emergencyCard: {
    flexDirection: 'row',
    backgroundColor: '#FFF5F5',
    marginHorizontal: 20,
    marginTop: 25,
    padding: 16,
    borderRadius: 12,
    borderLeftWidth: 4,
    borderLeftColor: '#FF3B30',
  },
  emergencyIconContainer: {
    marginRight: 12,
  },
  emergencyTextContainer: {
    flex: 1,
  },
  emergencyTitle: {
    fontSize: 15,
    fontWeight: '600',
    color: '#FF3B30',
    marginBottom: 6,
  },
  emergencyText: {
    fontSize: 13,
    color: '#666',
    lineHeight: 20,
  },
  emergencyPhone: {
    fontWeight: '600',
    color: '#FF3B30',
  },
  appInfoCard: {
    backgroundColor: '#fff',
    marginHorizontal: 20,
    marginTop: 25,
    padding: 20,
    borderRadius: 12,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },
  appInfoTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#1F1147',
    marginBottom: 5,
  },
  appInfoVersion: {
    fontSize: 13,
    color: '#999',
    marginBottom: 5,
  },
  appInfoCopyright: {
    fontSize: 12,
    color: '#999',
    marginBottom: 20,
  },
  socialLinks: {
    flexDirection: 'row',
    justifyContent: 'center',
    gap: 15,
  },
  socialButton: {
    width: 44,
    height: 44,
    borderRadius: 22,
    backgroundColor: '#F5F7FA',
    justifyContent: 'center',
    alignItems: 'center',
  },
  bottomPadding: {
    height: 50,
  },
});