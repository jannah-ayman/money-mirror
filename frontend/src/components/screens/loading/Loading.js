// Loading.js
import React from 'react';
import { View, Image, StyleSheet, Modal, Text } from 'react-native';

export default function Loading({ visible, message }) {
  return (
    <Modal
      transparent={true}
      animationType="fade"
      visible={visible}
    >
      <View style={styles.overlay}>
        <View style={styles.container}>
          <Image
            source={require('../../../../assets/images/loading.gif')}
            style={styles.gif}
          />
          {message && <Text style={styles.message}>{message}</Text>}
        </View>
      </View>
    </Modal>
  );
}

const styles = StyleSheet.create({
  overlay: {
    flex:1,
    backgroundColor: 'rgba(0,0,0,0.4)',
    justifyContent: 'center',
    alignItems: 'center',
  },
  container: {
    width:150,
    height:150,
    backgroundColor:'white',
    borderRadius:20,
    justifyContent:'center',
    alignItems:'center',
    padding:10,
  },
  gif: {
    width:80,
    height:80,
    marginBottom:10
  },
  message: {
    fontSize:14,
    textAlign:'center',
    color:'#333'
  }
});
