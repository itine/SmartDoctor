'use strict';
/*
config = {
        room:'testRoom',
        creator: true,
		pcConfig:{
            //iceTransports: 'relay',
            iceServers: [{
                'urls': 'stun:stun.l.google.com:19302'
            }],
        sdpConstraints : {
            offerToReceiveAudio: true,
            offerToReceiveVideo: true
        },
        localVideo : document.querySelector('#localVideo'),
        remoteVideo : document.querySelector('#remoteVideo'),
        signalingURL:  'wss:/'+ location.hostname+(location.port ? ':'+location.port: '')+'/rtc',
}
*/
function WebRTCLib(config){
    if(config == null) config = {};
    this.token = false;
    this.room = config.room || 'testRoom',
    //this.isChannelReady = false;
    this.isStarted = false;
    this.localStream = null;
    this.remoteStream= null;
    this.pcConfig = config.pcConfig || {
        //iceTransports: 'relay',
        iceServers: [{
            'urls': 'stun:stun.l.google.com:19302'
        }]
    };
    this.sdpConstraints = config.sdpConstraints || {
        offerToReceiveAudio: true,
        offerToReceiveVideo: true
    };
    this.localVideo = document.querySelector('#localVideo');
    this.remoteVideo = document.querySelector('#remoteVideo');


    this.signalingURL = config.signalingURL || 'wss:/'+ location.hostname+(location.port ? ':'+location.port: '')+'/rtc';

    this.creator = config.creator;
    this.channelReady = false;
    this.channel = null;
    this.name = "";

    this.fileDataChannel = null;
    //this.sendFile = false;
}

WebRTCLib.prototype.connect = function(){
    this.channelReady = false;
    var rtcOBJ = this;
    if(this.token){
        this.channel = new WebSocket(this.signalingURL+"?token=" + this.token.token);
        this.channel.onopen = function(){
            rtcOBJ.channelReady = true;
            if(rtcOBJ.creator){
                rtcOBJ.sendMessage({"type" : "CREATEROOM", "value" : rtcOBJ.room });
                //doCall();
            }else{
                rtcOBJ.sendMessage({"type" : "ENTERROOM", "value" : rtcOBJ.room });
            }
        };
        this.channel.onmessage = function(message){
                var data = message.data;
                if(data instanceof ArrayBuffer) {
                    console.log('ArrayBuffer data');
                }

                if(data instanceof Blob) {
                    console.log('Blob data');
                }

                if(data instanceof String) {
                    console.log('Text data');
                }

                if(typeof data === 'string') {
                    console.log('Text data');
                }
            rtcOBJ._processSignalingMessage(data);
        };
        this.channel.onclose = function(){
            rtcOBJ.disconnect();
        };
    }

}

WebRTCLib.prototype._processSignalingMessage = function(message) {
    var msg = JSON.parse(message);

    if (msg.type === 'offer') {
        this.pc.setRemoteDescription(new RTCSessionDescription(msg));
        this._doAnswer();
    } else if (msg.type === 'answer') {
        this.pc.setRemoteDescription(new RTCSessionDescription(msg));
    } else if (msg.type === 'candidate') {
        var candidate = new RTCIceCandidate({sdpMLineIndex:msg.label, candidate:msg.candidate});
        this.pc.addIceCandidate(candidate);
    } else if (msg.type === 'GETROOM') {
        this.onGetRoom(msg);
    } else if (msg.type === 'CREATEROOM') {
        this.onCreateRoom(msg)
    } else if (msg.type === 'ENTERROOM') {
        this.onEnterRoom(msg);
    } else if (msg.type === 'ASKSTREAM') {
        this._doCall();
    } else if (msg.type === 'CLOSEROOM') {
        this.disconnect();
    }else if (msg.type === 'WRONGROOM') {
       this.onWrongRoom(msg);
    }else if (msg.type === 'TEXTMESSAGE') {
            this.onTextMessage(msg);
    }
}

WebRTCLib.prototype.getToken = function(url){
    this.token = false;
    var rtcOBJ = this;
    let xhr = new XMLHttpRequest();
    xhr.open("GET", url, true);
    xhr.onreadystatechange = function(){
        if(xhr.readyState === XMLHttpRequest.DONE && xhr.status === 200) {
            rtcOBJ.token = JSON.parse(xhr.responseText);
            rtcOBJ.onTokenRecived();
        };
    };
    xhr.send();
}

WebRTCLib.prototype._get_token_from_signaling_server = function(url, name, role, pass){
        this.token = false;

        var rtcOBJ = this;

        this.name = name;

        let xhr = new XMLHttpRequest();

        let body = JSON.stringify({
            'login': name,
            'password_hash':pass,
            'role':role
        });

        xhr.open("POST", url, true);
        xhr.setRequestHeader('Content-Type', 'application/json');

        xhr.onreadystatechange = function(){
            if(xhr.readyState === XMLHttpRequest.DONE && xhr.status === 200) {
                rtcOBJ.token = JSON.parse(xhr.responseText);
                rtcOBJ.onTokenRecived();
            };
        };
        xhr.onerror = function(){
            console.log(xhr);
        }
        xhr.send(body);
}

// Events

WebRTCLib.prototype.onTokenRecived = function(){
    this.connect();
}

WebRTCLib.prototype.onGetRoom = function(msg){
    this.room = msg.value;
}

WebRTCLib.prototype.onCreateRoom = function(msg){
        this.room = msg.value;
        this._createPeerConnection();
}

WebRTCLib.prototype.onWrongRoom = function(msg){
        console.log("Wrong room: " + msg.value);
}

WebRTCLib.prototype.onEnterRoom = function(msg){
        this.room = msg.value;
        this._createPeerConnection();
        this.sendMessage({
            type: 'ASKSTREAM',
            value: this.room
        });
}

WebRTCLib.prototype.onTextMessage = function(msg){
        console.log("TextMessage from user " + msg.fromUser + ": " + msg.value);
}

// Events

WebRTCLib.prototype._createPeerConnection = function(){
    try {
        var rtcOBJ = this;
        this.pc = new RTCPeerConnection(this.pcConfig);
        this.pc.onicecandidate = function(event) {
                                   console.log('icecandidate event: ', event.candidate);
                                   if (event.candidate) {
                                     rtcOBJ.sendMessage({
                                       type: 'candidate',
                                       label: event.candidate.sdpMLineIndex,
                                       id: event.candidate.sdpMid,
                                       candidate: event.candidate.candidate
                                     });
                                   } else {
                                     console.log('End of candidates.');
                                   }
                                 };
        this.pc.onaddstream = function(event) {
                                  console.log( "On addstream: " + event);
                                  rtcOBJ.remoteStream = event.stream;
                                  rtcOBJ.remoteVideo.srcObject = rtcOBJ.remoteStream;

                              };
        this.pc.onremovestream = function(event) {
                                   console.log('Remote stream removed. Event: ', event);
                                 };
        this.pc.onsignalingstatechange = function(event) {
                                             if(rtcOBJ.pc!=null){
                                                console.log( "Signaling state changed: " + rtcOBJ.pc.signalingState);
                                                switch(rtcOBJ.pc.signalingState) {
                                                    case "stable":
                                                    //if(rtcOBJ.localStream==null) rtcOBJ.startStream();
                                                    //updateStatus("ICE negotiation complete");
                                                    break;
                                                }
                                             }

                                         };
        this.pc.onnegotiationneeded = function () {
            //console.log('onnegotiationneeded');
            rtcOBJ._doCall();
        };

        this.pc.ondatachannel = function(event){
                console.log(event);
                //listenForMessages(event.channel);
        };
        //console.log('Created RTCPeerConnnection');
        //this.startStream();
        if(this.creator)
            this.createDataChannel();

      } catch (e) {
        console.log('Failed to create PeerConnection, exception: ' + e.message);
        //alert('Cannot create RTCPeerConnection object.');
        return;
      }
}


WebRTCLib.prototype._handleOnTrack = function(event) {
    console.log( "On track: " + event);
    console.log( "On track kind: " + event.track.kind);
    if(event.track.remote && event.track.kind=="video"){
        this.remoteStream = event.streams[0];
        this.remoteVideo.srcObject = this.remoteStream;
    }
}



WebRTCLib.prototype._handleCreateOfferError = function(event) {
  console.log('createOffer() error: ', event);
}

WebRTCLib.prototype._doCall = function() {
  var rtcOBJ = this;
  console.log('Sending offer to peer');
  this.pc.createOffer(
        (sessionDescription) => rtcOBJ._setLocalAndSendMessage(sessionDescription),
        (error) => rtcOBJ._onCreateSessionDescriptionError(error)
  );
}

WebRTCLib.prototype._doAnswer = function() {
  console.log('Sending answer to peer.');
  var rtcOBJ = this;
  this.pc.createAnswer().then(
    (sessionDescription) => rtcOBJ._setLocalAndSendMessage(sessionDescription),
    (error) => rtcOBJ._onCreateSessionDescriptionError(error)
  );
}

WebRTCLib.prototype._setLocalAndSendMessage = function(sessionDescription) {
  console.log(this);
  this.pc.setLocalDescription(sessionDescription);
  console.log('setLocalAndSendMessage sending message', sessionDescription);
  this.sendMessage(sessionDescription);
}

WebRTCLib.prototype._onCreateSessionDescriptionError = function(error) {
  console.log('Failed to create session description: ' + error.toString());
}

WebRTCLib.prototype.hangup = function() {
  console.log('Hanging up.');
  if(this.creator){
    this.sendMessage({"type" : "CLOSEROOM", "value" : this.room, "fromUser": this.name });
  }else{
    this.sendMessage({"type" : "LEAVEROOM", "value" : this.room, "fromUser": this.name });
    this.disconnect();
  }


}

WebRTCLib.prototype.disconnect = function(){
    this.stopStream();
    this.isStarted = false;
    if(this.pc!=null) this.pc.close();
    this.pc = null;
    this.channelReady = false;
    if(this.channel!=null) this.channel.close();
    this.channel = null;
    this.room = "";
}

WebRTCLib.prototype.sendMessage = function(message) {
    if(this.channelReady)
        this.channel.send(JSON.stringify(message));
};

WebRTCLib.prototype.createDataChannel = function(channelName) {
        this.fileDataChannel = this.pc.createDataChannel("File transfer");
        this.fileDataChannel.binarytype = 'arraybuffer';
        var rtcOBJ = this;
        this.pc.createOffer().then(
            descr => {
                rtcOBJ.pc.setLocalDescription(descr);
                rtcOBJ.channel.send(JSON.stringify(descr));
                //this.sendFile = true;
                console.log(descr);
            }
        );
};

WebRTCLib.prototype.startStream = function(){
    var rtcOBJ = this;
    var gotStream = function(stream){
        console.log('Adding local stream.');
        rtcOBJ.isStarted = true;
        rtcOBJ.localStream = stream;
        rtcOBJ.localVideo.srcObject = rtcOBJ.localStream;
        //sendMessage('got user media');
        rtcOBJ.pc.addStream(rtcOBJ.localStream);
    }
    navigator.getWebcam = (navigator.getUserMedia || navigator.webKitGetUserMedia || navigator.moxGetUserMedia || navigator.mozGetUserMedia || navigator.msGetUserMedia);
    if (navigator.mediaDevices) {
        navigator.mediaDevices.getUserMedia({  audio: true, video: true })
        .then(gotStream)
        .catch(function(e) {
            console.log('getUserMedia() error: ' + e.name);
        });
    }
    else {
    navigator.getWebcam({ audio: true, video: true },
         gotStream,
         function(e) {console.log('getUserMedia() error: ' + e.name);});
    }
}

WebRTCLib.prototype.stopStream  = function(){
    this.localStream.getTracks().forEach(track => track.stop());
    //this.localStream = null;
    //this.remoteStream = null;
    this.localVideo.srcObject = null;
    this.remoteVideo.srcObject = null;
}



WebRTCLib.prototype.sendTextMessage  = function(message){
    this.sendMessage({"type" : "TEXTMESSAGE", "value" : message, "fromUser": this.name });
}




