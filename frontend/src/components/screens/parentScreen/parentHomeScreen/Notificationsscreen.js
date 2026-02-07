import React, { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  StatusBar,
  Alert,
} from 'react-native';
import { useNavigation } from '@react-navigation/native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';

export default function NotificationsScreen() {
  const navigation = useNavigation();

  const [notifications, setNotifications] = useState([
    {
      id: 1,
      type: 'expense',
      icon: 'cart',
      iconColor: '#FF6B9D',
      title: 'New Expense Logged',
      message: 'Jake spent 50 EGP on snacks',
      time: '5 minutes ago',
      isRead: false,
      date: 'Today',
    },
    {
      id: 2,
      type: 'goal',
      icon: 'trophy',
      iconColor: '#FFB800',
      title: 'Goal Progress',
      message: 'Sarah is 80% closer to her bicycle goal!',
      time: '1 hour ago',
      isRead: false,
      date: 'Today',
    },
    {
      id: 3,
      type: 'allowance',
      icon: 'cash',
      iconColor: '#00D4AA',
      title: 'Allowance Reminder',
      message: 'Weekly allowance is due tomorrow',
      time: '2 hours ago',
      isRead: false,
      date: 'Today',
    },
    {
      id: 4,
      type: 'achievement',
      icon: 'medal',
      iconColor: '#5F3BFF',
      title: 'Achievement Unlocked',
      message: 'Jake earned "Smart Saver" badge!',
      time: '3 hours ago',
      isRead: true,
      date: 'Today',
    },
    {
      id: 5,
      type: 'expense',
      icon: 'cart',
      iconColor: '#FF6B9D',
      title: 'Purchase Alert',
      message: 'Sarah bought a book for 120 EGP',
      time: '5 hours ago',
      isRead: true,
      date: 'Today',
    },
    {
      id: 6,
      type: 'goal',
      icon: 'flag',
      iconColor: '#FFB800',
      title: 'Goal Achieved',
      message: 'Congratulations! Jake reached his goal',
      time: 'Yesterday',
      isRead: true,
      date: 'Yesterday',
    },
    {
      id: 7,
      type: 'login',
      icon: 'log-in',
      iconColor: '#00D4AA',
      title: 'Login Activity',
      message: 'Sarah logged in from her device',
      time: 'Yesterday',
      isRead: true,
      date: 'Yesterday',
    },
    {
      id: 8,
      type: 'report',
      icon: 'bar-chart',
      iconColor: '#5F3BFF',
      title: 'Weekly Report Ready',
      message: 'Your weekly expense report is available',
      time: '2 days ago',
      isRead: true,
      date: 'This Week',
    },
    {
      id: 9,
      type: 'reminder',
      icon: 'notifications',
      iconColor: '#FF6B9D',
      title: 'Payment Reminder',
      message: 'Monthly allowance payment scheduled',
      time: '3 days ago',
      isRead: true,
      date: 'This Week',
    },
    {
      id: 10,
      type: 'system',
      icon: 'information-circle',
      iconColor: '#00D4AA',
      title: 'App Update Available',
      message: 'Version 1.1.0 is ready to download',
      time: '4 days ago',
      isRead: true,
      date: 'This Week',
    },
  ]);

  const [filter, setFilter] = useState('all'); // all, unread, read

  const unreadCount = notifications.filter(n => !n.isRead).length;

  const handleMarkAsRead = (id) => {
    setNotifications(notifications.map(notif => 
      notif.id === id ? { ...notif, isRead: true } : notif
    ));
  };

  const handleMarkAllAsRead = () => {
    setNotifications(notifications.map(notif => ({ ...notif, isRead: true })));
  };

  const handleDeleteNotification = (id) => {
    Alert.alert(
      'Delete Notification',
      'Are you sure you want to delete this notification?',
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Delete',
          style: 'destructive',
          onPress: () => {
            setNotifications(notifications.filter(notif => notif.id !== id));
          },
        },
      ]
    );
  };

  const handleClearAll = () => {
    Alert.alert(
      'Clear All Notifications',
      'This will delete all notifications. Are you sure?',
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Clear All',
          style: 'destructive',
          onPress: () => {
            setNotifications([]);
          },
        },
      ]
    );
  };

  const getFilteredNotifications = () => {
    if (filter === 'unread') {
      return notifications.filter(n => !n.isRead);
    } else if (filter === 'read') {
      return notifications.filter(n => n.isRead);
    }
    return notifications;
  };

  const groupNotificationsByDate = () => {
    const filtered = getFilteredNotifications();
    const grouped = {};
    
    filtered.forEach(notif => {
      if (!grouped[notif.date]) {
        grouped[notif.date] = [];
      }
      grouped[notif.date].push(notif);
    });
    
    return grouped;
  };

  const renderNotificationItem = (item) => (
    <TouchableOpacity
      key={item.id}
      style={[styles.notificationCard, !item.isRead && styles.unreadCard]}
      onPress={() => handleMarkAsRead(item.id)}
      activeOpacity={0.7}
    >
      <View style={styles.notificationContent}>
        <View style={[styles.iconContainer, { backgroundColor: item.iconColor + '20' }]}>
          <Ionicons name={item.icon} size={24} color={item.iconColor} />
        </View>
        
        <View style={styles.notificationTextContainer}>
          <View style={styles.notificationHeader}>
            <Text style={styles.notificationTitle}>{item.title}</Text>
            {!item.isRead && <View style={styles.unreadDot} />}
          </View>
          <Text style={styles.notificationMessage}>{item.message}</Text>
          <Text style={styles.notificationTime}>{item.time}</Text>
        </View>
      </View>

      <TouchableOpacity
        style={styles.deleteButton}
        onPress={() => handleDeleteNotification(item.id)}
      >
        <Ionicons name="close-circle" size={20} color="#999" />
      </TouchableOpacity>
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
        <View style={styles.headerTop}>
          <TouchableOpacity
            style={styles.backButton}
            onPress={() => navigation.goBack()}
          >
            <Ionicons name="arrow-back" size={24} color="#fff" />
          </TouchableOpacity>
          <View style={styles.headerTitleContainer}>
            <Text style={styles.headerTitle}>Notifications</Text>
            {unreadCount > 0 && (
              <View style={styles.headerBadge}>
                <Text style={styles.headerBadgeText}>{unreadCount}</Text>
              </View>
            )}
          </View>
          <TouchableOpacity
            style={styles.settingsButton}
            onPress={() => navigation.navigate('ParentHomeStack', {
           screen: 'NotificationSettingsScreen',
          })}
          >
            <Ionicons name="settings-outline" size={24} color="#fff" />
          </TouchableOpacity>
        </View>

        {/* Action Buttons */}
        <View style={styles.actionButtons}>
          <TouchableOpacity
            style={styles.actionButton}
            onPress={handleMarkAllAsRead}
            disabled={unreadCount === 0}
          >
            <Ionicons 
              name="checkmark-done" 
              size={18} 
              color={unreadCount === 0 ? '#999' : '#fff'} 
            />
            <Text style={[styles.actionButtonText, unreadCount === 0 && styles.disabledText]}>
              Mark all read
            </Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={styles.actionButton}
            onPress={handleClearAll}
            disabled={notifications.length === 0}
          >
            <Ionicons 
              name="trash-outline" 
              size={18} 
              color={notifications.length === 0 ? '#999' : '#fff'} 
            />
            <Text style={[styles.actionButtonText, notifications.length === 0 && styles.disabledText]}>
              Clear all
            </Text>
          </TouchableOpacity>
        </View>
      </LinearGradient>

      {/* Filter Tabs */}
      <View style={styles.filterContainer}>
        <TouchableOpacity
          style={[styles.filterTab, filter === 'all' && styles.activeFilterTab]}
          onPress={() => setFilter('all')}
        >
          <Text style={[styles.filterText, filter === 'all' && styles.activeFilterText]}>
            All ({notifications.length})
          </Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={[styles.filterTab, filter === 'unread' && styles.activeFilterTab]}
          onPress={() => setFilter('unread')}
        >
          <Text style={[styles.filterText, filter === 'unread' && styles.activeFilterText]}>
            Unread ({unreadCount})
          </Text>
          {unreadCount > 0 && <View style={styles.filterBadge} />}
        </TouchableOpacity>

        <TouchableOpacity
          style={[styles.filterTab, filter === 'read' && styles.activeFilterTab]}
          onPress={() => setFilter('read')}
        >
          <Text style={[styles.filterText, filter === 'read' && styles.activeFilterText]}>
            Read ({notifications.length - unreadCount})
          </Text>
        </TouchableOpacity>
      </View>

      {/* Notifications List */}
      <ScrollView
        style={styles.scrollView}
        contentContainerStyle={styles.scrollContent}
        showsVerticalScrollIndicator={false}
      >
        {getFilteredNotifications().length === 0 ? (
          <View style={styles.emptyContainer}>
            <View style={styles.emptyIconContainer}>
              <Ionicons name="notifications-off" size={80} color="#E0E0E0" />
            </View>
            <Text style={styles.emptyTitle}>No Notifications</Text>
            <Text style={styles.emptyText}>
              {filter === 'unread' 
                ? "You're all caught up! No unread notifications."
                : filter === 'read'
                ? "No read notifications yet."
                : "You don't have any notifications yet."}
            </Text>
          </View>
        ) : (
          Object.entries(groupNotificationsByDate()).map(([date, items]) => (
            <View key={date} style={styles.dateGroup}>
              <Text style={styles.dateHeader}>{date}</Text>
              {items.map(item => renderNotificationItem(item))}
            </View>
          ))
        )}

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
    paddingTop: 50,
    paddingBottom: 20,
    paddingHorizontal: 20,
  },
  headerTop: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
  },
  backButton: {
    padding: 8,
  },
  headerTitleContainer: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  headerTitle: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#fff',
  },
  headerBadge: {
    backgroundColor: '#FF6B9D',
    borderRadius: 12,
    paddingHorizontal: 8,
    paddingVertical: 2,
    marginLeft: 8,
    minWidth: 24,
    alignItems: 'center',
  },
  headerBadgeText: {
    fontSize: 12,
    fontWeight: 'bold',
    color: '#fff',
  },
  settingsButton: {
    padding: 8,
  },
  actionButtons: {
    flexDirection: 'row',
    justifyContent: 'flex-end',
    marginTop: 15,
    gap: 15,
  },
  actionButton: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 6,
  },
  actionButtonText: {
    fontSize: 13,
    color: '#fff',
    fontWeight: '500',
  },
  disabledText: {
    color: '#999',
  },
  filterContainer: {
    flexDirection: 'row',
    backgroundColor: '#fff',
    paddingHorizontal: 20,
    paddingVertical: 12,
    gap: 10,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },
  filterTab: {
    flex: 1,
    paddingVertical: 10,
    paddingHorizontal: 12,
    borderRadius: 8,
    backgroundColor: '#F5F7FA',
    alignItems: 'center',
    flexDirection: 'row',
    justifyContent: 'center',
  },
  activeFilterTab: {
    backgroundColor: '#5F3BFF',
  },
  filterText: {
    fontSize: 13,
    fontWeight: '500',
    color: '#666',
  },
  activeFilterText: {
    color: '#fff',
  },
  filterBadge: {
    width: 8,
    height: 8,
    borderRadius: 4,
    backgroundColor: '#FF6B9D',
    marginLeft: 6,
  },
  scrollView: {
    flex: 1,
  },
  scrollContent: {
    paddingBottom: 20,
  },
  dateGroup: {
    marginTop: 20,
  },
  dateHeader: {
    fontSize: 14,
    fontWeight: '600',
    color: '#666',
    marginBottom: 12,
    marginLeft: 20,
    textTransform: 'uppercase',
    letterSpacing: 0.5,
  },
  notificationCard: {
    flexDirection: 'row',
    alignItems: 'flex-start',
    justifyContent: 'space-between',
    backgroundColor: '#fff',
    marginHorizontal: 20,
    marginBottom: 10,
    padding: 16,
    borderRadius: 12,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },
  unreadCard: {
    borderLeftWidth: 4,
    borderLeftColor: '#5F3BFF',
    backgroundColor: '#FAFBFF',
  },
  notificationContent: {
    flexDirection: 'row',
    flex: 1,
  },
  iconContainer: {
    width: 48,
    height: 48,
    borderRadius: 24,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 12,
  },
  notificationTextContainer: {
    flex: 1,
  },
  notificationHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 4,
  },
  notificationTitle: {
    fontSize: 15,
    fontWeight: '600',
    color: '#1F1147',
    flex: 1,
  },
  unreadDot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    backgroundColor: '#5F3BFF',
    marginLeft: 8,
  },
  notificationMessage: {
    fontSize: 14,
    color: '#666',
    lineHeight: 20,
    marginBottom: 6,
  },
  notificationTime: {
    fontSize: 12,
    color: '#999',
  },
  deleteButton: {
    padding: 4,
    marginLeft: 8,
  },
  emptyContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    paddingVertical: 80,
  },
  emptyIconContainer: {
    marginBottom: 20,
  },
  emptyTitle: {
    fontSize: 20,
    fontWeight: '600',
    color: '#1F1147',
    marginBottom: 8,
  },
  emptyText: {
    fontSize: 14,
    color: '#999',
    textAlign: 'center',
    lineHeight: 20,
    paddingHorizontal: 40,
  },
  bottomPadding: {
    height: 20,
  },
});