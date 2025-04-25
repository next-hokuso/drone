var FirebasePlugin = {
    FB_CreateEmailVerifiedJS__deps: ['UserReload'],
    
    Hello: function () {
      window.alert("Hello, world!");
    },
    
    HelloString: function (str) {
      window.alert(Pointer_stringify(str));
    },
    
    PrintFloatArray: function (array, size) {
      for(var i = 0; i < size; i++)
      console.log(HEAPF32[(array >> 2) + i]);
    },
    
    AddNumbers: function (x, y) {
      return x + y;
    },
    
    StringReturnValueFunction: function () {
      var returnStr = "bla";
      var bufferSize = lengthBytesUTF8(returnStr) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(returnStr, buffer, bufferSize);
      return buffer;
    },
    
    BindWebGLTexture: function (texture) {
      GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[texture]);
    },
    
    CheckMobilePlatformJS: function () {
        var ua = window.navigator.userAgent.toLowerCase();
        
        //var mobilePattern = /android|iphone|ipad|ipod/i;
        //return ua.search(mobilePattern) !== -1 || (ua.indexOf("macintosh") !== -1 && "ontouchend" in document);
        
        //ua = "Mozilla/5.0 (iPhone; CPU iPhone OS 17_5_1 like Max OS X)";
        //ua = "Mozilla/5.0 (Linux; Android 7.0; WAS-LX2J) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.116 Mobile Safari/537.36";
        
        //alert(ua);
        
        var androidOSPattern = /android [0-9]+/;
        var androidPadPattern = /linux x[0-9]+_[0-9]+/;
        var iPhoneOSPattern = /iphone os [0-9]+_[0-9]+_[0-9]+/;
        var iPadOSPattern = /ipad os [0-9]+_[0-9]+_[0-9]+/;
        var iPodOSPattern = /ipod os [0-9]+_[0-9]+_[0-9]+/;
        
        var ret = ua.match(androidOSPattern);
        if (ret == null) {
            ret = ua.match(androidPadPattern);
            if (ret == null)
            {
                ret = ua.match(iPhoneOSPattern);
                if (ret == null) {
                    ret = ua.match(iPadOSPattern);
                    if (ret == null) {
                        ret = ua.match(iPodOSPattern);
                    }
                }
            }
        }
      
        if (ret == null) {
            return null;
        }
        
        var bufferSize = lengthBytesUTF8(ret[0]) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(ret[0], buffer, bufferSize);
        return buffer;
        
        //
        ////console.log(window.navigator.userAgent);
        //alert(window.navigator.userAgent);
        //
        //if (window.navigator.userAgent.match(/iPhone|Android.+Mobile/)) {
        //    return true;
        //} else {
        //    return false;
        //}
    },
    
    FB_InitializeAppJS: function ()
    {
        const firebaseConfig = {
            // develop
            apiKey: "AIzaSyA3YeUG0tpstsPBMdkulk-IcbDyoYV0H_E",
            authDomain: "drone-star-training-dev.firebaseapp.com",
            projectId: "drone-star-training-dev",
            storageBucket: "drone-star-training-dev.appspot.com",
            messagingSenderId: "128999627223",
            appId: "1:128999627223:web:b4134357540a3e58a35392",
            measurementId: "G-L4HTJJKDBR"
            // master
            //apiKey: "AIzaSyDPHI8BzH93wrUBF6B99gLmJfgc2uRBQ50",
            //authDomain: "drone-star-training-web.firebaseapp.com",
            //projectId: "drone-star-training-web",
            //storageBucket: "drone-star-training-web.appspot.com",
            //messagingSenderId: "162503327460",
            //appId: "1:162503327460:web:3141a98dbb01df4a15f6f1",
            //measurementId: "G-CYDBG692G4"
        };
        if (firebase.apps.length === 0) {
            window.firebase.initializeApp(firebaseConfig);
        }
    },
    
    FB_SignInJS: function (instanceID, callback, emailPtr, passwordPtr)
    {
        var email = UTF8ToString(emailPtr);
        var password = UTF8ToString(passwordPtr);
        firebase.auth().signInWithEmailAndPassword(email, password)
            .then((userCredential) => {
                //window.alert("Login Success");
                var user = userCredential.user;
                var receiveUserClass = {
                    displayName: user.displayName,      // string
                    email: user.email,                  // string
                    uid: user.uid,                      // string
                };
                // serialize data1 to json
                var json = JSON.stringify(receiveUserClass);
                // calculate buffer size
                var bufferSize = lengthBytesUTF8(json) + 1;
                // allocate buffer memory
                var buffer = _malloc(bufferSize);
                // convert string to UTF8
                stringToUTF8(json, buffer, bufferSize);
                // execute callback from c# defined
                Module.dynCall_viii(callback, instanceID, buffer, true);
            })
            .catch((error) => {
                //window.alert(error.code);
                // calculate buffer size
                var errorStr = error.code.toString();
                var bufferSize = lengthBytesUTF8(errorStr) + 1;
                // allocate buffer memory
                var buffer = _malloc(bufferSize);
                // convert string to UTF8
                stringToUTF8(errorStr, buffer, bufferSize);
                // execute callback from c# defined
                Module.dynCall_viii(callback, instanceID, buffer, false);
            });
    },
    
    FB_RegisterUserJS: function (instanceID, callback, emailPtr, passwordPtr)
    {
        var email = UTF8ToString(emailPtr);
        var password = UTF8ToString(passwordPtr);
        firebase.auth().createUserWithEmailAndPassword(email, password)
            .then((userCredential) => {
                // Signed in 
                var user = userCredential.user;
                var receiveUserClass = {
                    displayName: user.displayName,      // string
                    email: user.email,                  // string
                    uid: user.uid,                      // string
                };
                // serialize data1 to json
                var json = JSON.stringify(receiveUserClass);
                // calculate buffer size
                var bufferSize = lengthBytesUTF8(json) + 1;
                // allocate buffer memory
                var buffer = _malloc(bufferSize);
                // convert string to UTF8
                stringToUTF8(json, buffer, bufferSize);
                // execute callback from c# defined
                Module.dynCall_viii(callback, instanceID, buffer, true);
            })
            .catch((error) => {
                //window.alert(error.code);
                // calculate buffer size
                var errorStr = error.code.toString();
                var bufferSize = lengthBytesUTF8(errorStr) + 1;
                // allocate buffer memory
                var buffer = _malloc(bufferSize);
                // convert string to UTF8
                stringToUTF8(errorStr, buffer, bufferSize);
                // execute callback from c# defined
                Module.dynCall_viii(callback, instanceID, buffer, false);
            });
    },
    
    FB_CreateUserInfoStorageJS: function (instanceID, callback, uidPtr, genderPtr, agePtr, workPtr)
    {
        var uid = UTF8ToString(uidPtr);
        var gender = UTF8ToString(genderPtr);
        var age = UTF8ToString(agePtr);
        var work = UTF8ToString(workPtr);
        
        var storageRef = firebase.storage().ref();
        var userRef = storageRef.child("user/" + uid + "/");
        var userInfoRef = userRef.child("profile.json");
        
        var receiveUserStorageClass = {
            gender: gender,
            age: age,
            work: work
        };
        var json = JSON.stringify(receiveUserStorageClass);
        
        userInfoRef.putString(json).then((snapShot) => {
            // serialize receiveUserStorageClass to json
            var json = JSON.stringify(receiveUserStorageClass);
            // calculate buffer size
            var bufferSize = lengthBytesUTF8(json) + 1;
            // allocate buffer memory
            var buffer = _malloc(bufferSize);
            // convert string to UTF8
            stringToUTF8(json, buffer, bufferSize);
            // execute callback from c# defined
            Module.dynCall_viii(callback, instanceID, buffer, true);
        })
        .catch((error) => {
            // error
            // calculate buffer size
            var errorStr = error.code.toString();
            var bufferSize = lengthBytesUTF8(errorStr) + 1;
            // allocate buffer memory
            var buffer = _malloc(bufferSize);
            // convert string to UTF8
            stringToUTF8(errorStr, buffer, bufferSize);
            // execute callback from c# defined
            Module.dynCall_viii(callback, instanceID, buffer, false);
        });
    },
    
    FB_GetUserInfoStorageJS: function (instanceID, callback, uidPtr)
    {
        var uid = UTF8ToString(uidPtr);
        
        var storageRef = firebase.storage().ref();
        var userRef = storageRef.child("user/" + uid + "/");
        var userInfoRef = userRef.child("profile.json");
        
        userInfoRef.getDownloadURL()
            .then((url) => {
                // `url` is the download URL for xxxx
        
                // This can be downloaded directly:
                var xhr = new XMLHttpRequest();
                xhr.responseType = 'json';
                xhr.onload = (event) => {
                    var json = JSON.stringify(xhr.response);
                    //console.log("json=" + json);
        
                    // calculate buffer size
                    var bufferSize = lengthBytesUTF8(json) + 1;
                    // allocate buffer memory
                    var buffer = _malloc(bufferSize);
                    // convert string to UTF8
                    stringToUTF8(json, buffer, bufferSize);
                    Module.dynCall_viii(callback, instanceID, buffer, true);
                };
                xhr.open('GET', url);
                xhr.send();
                //console.log("url="+url);
            })
            .catch((error) => {
                // error
                // calculate buffer size
                var errorStr = error.code.toString();
                var bufferSize = lengthBytesUTF8(errorStr) + 1;
                // allocate buffer memory
                var buffer = _malloc(bufferSize);
                // convert string to UTF8
                stringToUTF8(errorStr, buffer, bufferSize);
                // execute callback from c# defined
                Module.dynCall_viii(callback, instanceID, buffer, false);
            });
    },
    
    FB_DeleteUserInfoStorageJS: function (instanceID, callback, uidPtr)
    {
        var uid = UTF8ToString(uidPtr);
        var storageRef = firebase.storage().ref();
        var listRef = storageRef.child("user/" + uid);

        listRef.listAll()
            .then((res) => {
                var count = res.items.length;
                console.log("delete item count = " + count);
                res.items.forEach((itemRef) => {
                    itemRef.delete().then(() => {
                        console.log("deleted:" + itemRef.name);
                        if (--count == 0) {
                            console.log("delete finished");
                            Module.dynCall_viii(callback, instanceID, null, true);
                        }
                    }).catch((error) => {
                        console.log("delete_error:" + itemRef.name);
                        console.log("error_code:" + error.code.toString());
                        if (--count == 0) {
                            console.log("delete finished");
                            Module.dynCall_viii(callback, instanceID, null, true);
                        }
                    });
                });
            }).catch((error) => {
                var errorStr = error.code.toString();
                var bufferSize = lengthBytesUTF8(errorStr) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(errorStr, buffer, bufferSize);
                Module.dynCall_viii(callback, instanceID, buffer, false);
            });

      //var userRef = storageRef.child("user/" + uid + "/");
      //var userInfoRef = userRef.child("profile.json");
      //
      //userInfoRef.delete()
      //    .then(() => {
      //        // success
      //        Module.dynCall_viii(callback, instanceID, null, true);
      //    })
      //    .catch((error) => {
      //        // error
      //        // calculate buffer size
      //        var errorStr = error.code.toString();
      //        var bufferSize = lengthBytesUTF8(errorStr) + 1;
      //        // allocate buffer memory
      //        var buffer = _malloc(bufferSize);
      //        // convert string to UTF8
      //        stringToUTF8(errorStr, buffer, bufferSize);
      //        // execute callback from c# defined
      //        Module.dynCall_viii(callback, instanceID, buffer, false);
      //    });
    },
    
    FB_UpdateProfileJS: function (instanceID, callback, displayNamePtr, photoURLPtr)
    {
        var currentUser = firebase.auth().currentUser;
        if (currentUser != null)
        {
            var DisplayName = UTF8ToString(displayNamePtr);
            var PhotoURL = UTF8ToString(photoURLPtr);
            currentUser.updateProfile({
                displayName: DisplayName,
                photoURL: PhotoURL
            }).then(() => {
                // success
                var user = firebase.auth().currentUser;
                var receiveUserClass = {
                    displayName: user.displayName,      // string
                    email: user.email,                  // string
                    uid: user.uid,                      // string
                };
                // serialize data1 to json
                var json = JSON.stringify(receiveUserClass);
                // calculate buffer size
                var bufferSize = lengthBytesUTF8(json) + 1;
                // allocate buffer memory
                var buffer = _malloc(bufferSize);
                // convert string to UTF8
                stringToUTF8(json, buffer, bufferSize);
                Module.dynCall_viii(callback, instanceID, buffer, true);
            }).catch((error) => {
                // error
                // calculate buffer size
                var errorStr = error.code.toString();
                var bufferSize = lengthBytesUTF8(errorStr) + 1;
                // allocate buffer memory
                var buffer = _malloc(bufferSize);
                // convert string to UTF8
                stringToUTF8(errorStr, buffer, bufferSize);
                // execute callback from c# defined
                Module.dynCall_viii(callback, instanceID, buffer, false);
            });
        }
        else
        {
            var errorStr = "current user is null";
            var bufferSize = lengthBytesUTF8(errorStr) + 1;
            // allocate buffer memory
            var buffer = _malloc(bufferSize);
            // convert string to UTF8
            stringToUTF8(errorStr, buffer, bufferSize);
            // execute callback from c# defined
            Module.dynCall_viii(callback, instanceID, buffer, false);
        }
    },
    
    FB_SendEmailVerificationJS: function (instanceID, callback)
    {
        var user = firebase.auth().currentUser;
        if (user != null)
        {
            user.sendEmailVerification()
                .then(() => {
                    // Email verification sent!
                    var receiveUserClass = {
                        displayName: user.displayName,      // string
                        email: user.email,                  // string
                        uid: user.uid,                      // string
                    };
                    // serialize data1 to json
                    var json = JSON.stringify(receiveUserClass);
                    // calculate buffer size
                    var bufferSize = lengthBytesUTF8(json) + 1;
                    // allocate buffer memory
                    var buffer = _malloc(bufferSize);
                    // convert string to UTF8
                    stringToUTF8(json, buffer, bufferSize);
                    Module.dynCall_viii(callback, instanceID, buffer, true);
                });
        }
        else
        {
            var errorStr = "current user is null";
            var bufferSize = lengthBytesUTF8(errorStr) + 1;
            // allocate buffer memory
            var buffer = _malloc(bufferSize);
            // convert string to UTF8
            stringToUTF8(errorStr, buffer, bufferSize);
            // execute callback from c# defined
            Module.dynCall_viii(callback, instanceID, buffer, false);
        }
    },
    
    FB_DeleteUserJS: function (instanceID, callback, emailPtr, passwordPtr)
    {
        var user = firebase.auth().currentUser;
        if (user != null)
        {
            var email = UTF8ToString(emailPtr);
            var password = UTF8ToString(passwordPtr);
        
            // TODO(you): prompt the user to re-provide their sign-in credentials
            var credential = firebase.auth.EmailAuthProvider.credential(email, password);
            
            user.reauthenticateWithCredential(credential).then(() => {
                // User re-authenticated.
                user.delete().then(() => {
                    // User deleted. and signed out.
                    Module.dynCall_viii(callback, instanceID, null, true);
                }).catch((error) => {
                    // Error
                    var errorStr = error.code.toString();
                    var bufferSize = lengthBytesUTF8(errorStr) + 1;
                    var buffer = _malloc(bufferSize);
                    stringToUTF8(errorStr, buffer, bufferSize);
                    Module.dynCall_viii(callback, instanceID, buffer, false);
                });
            }).catch((error) => {
              // An error occurred
              // calculate buffer size
              var errorStr = error.code.toString();
              var bufferSize = lengthBytesUTF8(errorStr) + 1;
              // allocate buffer memory
              var buffer = _malloc(bufferSize);
              // convert string to UTF8
              stringToUTF8(errorStr, buffer, bufferSize);
              // execute callback from c# defined
              Module.dynCall_viii(callback, instanceID, buffer, false);
            });
        }
        else
        {
            var errorStr = "current user is null";
            var bufferSize = lengthBytesUTF8(errorStr) + 1;
            // allocate buffer memory
            var buffer = _malloc(bufferSize);
            // convert string to UTF8
            stringToUTF8(errorStr, buffer, bufferSize);
            // execute callback from c# defined
            Module.dynCall_viii(callback, instanceID, buffer, false);
        }
    },
    
    FB_VerifyBeforeUpdateEmailJS: function (instanceID, callback, emailPtr, passwordPtr, newEmailPtr)
    {
        var user = firebase.auth().currentUser;
        if (user != null)
        {
            var email = UTF8ToString(emailPtr);
            var password = UTF8ToString(passwordPtr);
            var newEmail = UTF8ToString(newEmailPtr);
        
            // TODO(you): prompt the user to re-provide their sign-in credentials
            var credential = firebase.auth.EmailAuthProvider.credential(email, password);
            
            user.reauthenticateWithCredential(credential).then(() => {
                // User re-authenticated.
                user.verifyBeforeUpdateEmail(newEmail).then(() => {
                  // Update successful
                  var receiveUserClass = {
                      displayName: user.displayName,      // string
                      email: user.email,                  // string
                      uid: user.uid,                      // string
                  };
                  // serialize data1 to json
                  var json = JSON.stringify(receiveUserClass);
                  // calculate buffer size
                  var bufferSize = lengthBytesUTF8(json) + 1;
                  // allocate buffer memory
                  var buffer = _malloc(bufferSize);
                  // convert string to UTF8
                  stringToUTF8(json, buffer, bufferSize);
                  Module.dynCall_viii(callback, instanceID, buffer, true);
                }).catch((error) => {
                  // An error occurred
                  // calculate buffer size
                  var errorStr = error.code.toString();
                  var bufferSize = lengthBytesUTF8(errorStr) + 1;
                  // allocate buffer memory
                  var buffer = _malloc(bufferSize);
                  // convert string to UTF8
                  stringToUTF8(errorStr, buffer, bufferSize);
                  // execute callback from c# defined
                  Module.dynCall_viii(callback, instanceID, buffer, false);
                });
            }).catch((error) => {
              // An error occurred
              // calculate buffer size
              var errorStr = error.code.toString();
              var bufferSize = lengthBytesUTF8(errorStr) + 1;
              // allocate buffer memory
              var buffer = _malloc(bufferSize);
              // convert string to UTF8
              stringToUTF8(errorStr, buffer, bufferSize);
              // execute callback from c# defined
              Module.dynCall_viii(callback, instanceID, buffer, false);
            });
        }
        else
        {
            var errorStr = "current user is null";
            var bufferSize = lengthBytesUTF8(errorStr) + 1;
            // allocate buffer memory
            var buffer = _malloc(bufferSize);
            // convert string to UTF8
            stringToUTF8(errorStr, buffer, bufferSize);
            // execute callback from c# defined
            Module.dynCall_viii(callback, instanceID, buffer, false);
        }
    },
    
    UserReload: async function () {
        await firebase.auth().currentUser.reload();
    },
    
    FB_CreateEmailVerifiedJS: function (instanceID, callback)
    {
        var user = firebase.auth().currentUser;
        if (user != null)
        {
            user.reload().then(() => {
                user = firebase.auth().currentUser;
                if (user != null && user.emailVerified) {
                    var receiveUserClass = {
                        displayName: user.displayName,
                        email: user.email,
                        uid: user.uid
                    };
                    var json = JSON.stringify(receiveUserClass);
                    var bufferSize = lengthBytesUTF8(json) + 1;
                    var buffer = _malloc(bufferSize);
                    stringToUTF8(json, buffer, bufferSize);
                    Module.dynCall_viii(callback, instanceID, buffer, true);
                } else {
                    var errorStr = "current user is not verified";
                    var bufferSize = lengthBytesUTF8(errorStr) + 1;
                    var buffer = _malloc(bufferSize);
                    stringToUTF8(errorStr, buffer, bufferSize);
                    Module.dynCall_viii(callback, instanceID, buffer, false);
                }
            }).catch(error => {
                var errorStr = error.code.toString();
                var bufferSize = lengthBytesUTF8(errorStr) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(errorStr, buffer, bufferSize);
                Module.dynCall_viii(callback, instanceID, buffer, false);
            });
        
            //_UserReload();
            //user = firebase.auth().currentUser;
            //if (user != null && user.emailVerified) {
            //    var receiveUserClass = {
            //        displayName: user.displayName,
            //        email: user.email,
            //        uid: user.uid
            //    };
            //    var json = JSON.stringify(receiveUserClass);
            //    var bufferSize = lengthBytesUTF8(json) + 1;
            //    var buffer = _malloc(bufferSize);
            //    stringToUTF8(json, buffer, bufferSize);
            //    Module.dynCall_viii(callback, instanceID, buffer, true);
            //}
            //else {
            //    var errorStr = "current user is not verified";
            //    var bufferSize = lengthBytesUTF8(errorStr) + 1;
            //    var buffer = _malloc(bufferSize);
            //    stringToUTF8(errorStr, buffer, bufferSize);
            //    Module.dynCall_viii(callback, instanceID, buffer, false);
            //}
        }
        else
        {
            var errorStr = "current user is null";
            var bufferSize = lengthBytesUTF8(errorStr) + 1;
            // allocate buffer memory
            var buffer = _malloc(bufferSize);
            // convert string to UTF8
            stringToUTF8(errorStr, buffer, bufferSize);
            // execute callback from c# defined
            Module.dynCall_viii(callback, instanceID, buffer, false);
        }
    },
    
    FB_GetCurrentUserJS: function (instanceID, callback)
    {
        var user = firebase.auth().currentUser;
        if (user != null)
        {
            var receiveUserClass = {
                displayName: user.displayName,
                email: user.email,
                uid: user.uid
            };
            var json = JSON.stringify(receiveUserClass);
            var bufferSize = lengthBytesUTF8(json) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(json, buffer, bufferSize);
            Module.dynCall_viii(callback, instanceID, buffer, true);
        }
        else
        {
            var errorStr = "current user is null";
            var bufferSize = lengthBytesUTF8(errorStr) + 1;
            // allocate buffer memory
            var buffer = _malloc(bufferSize);
            // convert string to UTF8
            stringToUTF8(errorStr, buffer, bufferSize);
            // execute callback from c# defined
            Module.dynCall_viii(callback, instanceID, buffer, false);
        }
    },
    
    FB_GetCurrentUserReloadJS: function (instanceID, callback)
    {
        var user = firebase.auth().currentUser;
        if (user != null)
        {
            user.reload().then(() => {
                user = firebase.auth().currentUser;
                if (user != null) {
                    var receiveUserClass = {
                        displayName: user.displayName,
                        email: user.email,
                        uid: user.uid
                    };
                    var json = JSON.stringify(receiveUserClass);
                    var bufferSize = lengthBytesUTF8(json) + 1;
                    var buffer = _malloc(bufferSize);
                    stringToUTF8(json, buffer, bufferSize);
                    Module.dynCall_viii(callback, instanceID, buffer, true);
                } else {
                    Module.dynCall_viii(callback, instanceID, null, false);
                }
            }).catch(error => {
                var errorStr = error.code.toString();
                var bufferSize = lengthBytesUTF8(errorStr) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(errorStr, buffer, bufferSize);
                Module.dynCall_viii(callback, instanceID, buffer, false);
            });
        }
        else
        {
            Module.dynCall_viii(callback, instanceID, null, false);
        }
    },
    
    FB_SignOutJS: function (instanceID, callback)
    {
        firebase.auth().signOut()
            .then(() => {
                // Sign-out successful.
                Module.dynCall_viii(callback, instanceID, null, true);
            })
            .catch((error) => {
                //window.alert(error.code);
                // calculate buffer size
                var errorStr = error.code.toString();
                var bufferSize = lengthBytesUTF8(errorStr) + 1;
                // allocate buffer memory
                var buffer = _malloc(bufferSize);
                // convert string to UTF8
                stringToUTF8(errorStr, buffer, bufferSize);
                // execute callback from c# defined
                Module.dynCall_viii(callback, instanceID, buffer, false);
            });
    },
    
    FB_UpdatePasswordJS: function (instanceID, callback, emailPtr, passwordPtr, newPasswordPtr)
    {
        var user = firebase.auth().currentUser;
        if (user != null)
        {
            var email = UTF8ToString(emailPtr);
            var password = UTF8ToString(passwordPtr);
            var newPassword = UTF8ToString(newPasswordPtr);
        
            // TODO(you): prompt the user to re-provide their sign-in credentials
            var credential = firebase.auth.EmailAuthProvider.credential(email, password);
            
            user.reauthenticateWithCredential(credential).then(() => {
                // User re-authenticated.
                user.updatePassword(newPassword).then(() => {
                    // Update successful.
                    var receiveUserClass = {
                        displayName: user.displayName,      // string
                        email: user.email,                  // string
                        uid: user.uid,                      // string
                    };
                    // serialize data1 to json
                    var json = JSON.stringify(receiveUserClass);
                    // calculate buffer size
                    var bufferSize = lengthBytesUTF8(json) + 1;
                    // allocate buffer memory
                    var buffer = _malloc(bufferSize);
                    // convert string to UTF8
                    stringToUTF8(json, buffer, bufferSize);
                    Module.dynCall_viii(callback, instanceID, buffer, true);
                }).catch((error) => {
                    // Error
                    var errorStr = error.code.toString();
                    var bufferSize = lengthBytesUTF8(errorStr) + 1;
                    var buffer = _malloc(bufferSize);
                    stringToUTF8(errorStr, buffer, bufferSize);
                    Module.dynCall_viii(callback, instanceID, buffer, false);
                });
            }).catch((error) => {
              // An error occurred
              // calculate buffer size
              var errorStr = error.code.toString();
              var bufferSize = lengthBytesUTF8(errorStr) + 1;
              // allocate buffer memory
              var buffer = _malloc(bufferSize);
              // convert string to UTF8
              stringToUTF8(errorStr, buffer, bufferSize);
              // execute callback from c# defined
              Module.dynCall_viii(callback, instanceID, buffer, false);
            });
        }
        else
        {
            var errorStr = "current user is null";
            var bufferSize = lengthBytesUTF8(errorStr) + 1;
            // allocate buffer memory
            var buffer = _malloc(bufferSize);
            // convert string to UTF8
            stringToUTF8(errorStr, buffer, bufferSize);
            // execute callback from c# defined
            Module.dynCall_viii(callback, instanceID, buffer, false);
        }
    },
    
    FB_ChangeEmailVerifiedJS: function (instanceID, callback, emailPtr, passwordPtr)
    {
        var user = firebase.auth().currentUser;
        if (user != null)
        {
            var email = UTF8ToString(emailPtr);
            var password = UTF8ToString(passwordPtr);
        
            // TODO(you): prompt the user to re-provide their sign-in credentials
            var credential = firebase.auth.EmailAuthProvider.credential(email, password);
            
            user.reauthenticateWithCredential(credential).then(() => {
                // User re-authenticated.
        
                var receiveUserClass = {
                    displayName: user.displayName,      // string
                    email: user.email,                  // string
                    uid: user.uid,                      // string
                };
                // serialize data1 to json
                var json = JSON.stringify(receiveUserClass);
                // calculate buffer size
                var bufferSize = lengthBytesUTF8(json) + 1;
                // allocate buffer memory
                var buffer = _malloc(bufferSize);
                // convert string to UTF8
                stringToUTF8(json, buffer, bufferSize);
                Module.dynCall_viii(callback, instanceID, buffer, true);
        
            }).catch((error) => {
              // An error occurred
              // calculate buffer size
              var errorStr = error.code.toString();
              var bufferSize = lengthBytesUTF8(errorStr) + 1;
              // allocate buffer memory
              var buffer = _malloc(bufferSize);
              // convert string to UTF8
              stringToUTF8(errorStr, buffer, bufferSize);
              // execute callback from c# defined
              Module.dynCall_viii(callback, instanceID, buffer, false);
            });
        }
        else
        {
            var errorStr = "current user is null";
            var bufferSize = lengthBytesUTF8(errorStr) + 1;
            // allocate buffer memory
            var buffer = _malloc(bufferSize);
            // convert string to UTF8
            stringToUTF8(errorStr, buffer, bufferSize);
            // execute callback from c# defined
            Module.dynCall_viii(callback, instanceID, buffer, false);
        }
    },
    
    FB_ReauthenticateWithCredentialJS: function (instanceID, callback, emailPtr, passwordPtr)
    {
        var user = firebase.auth().currentUser;
        if (user != null)
        {
            var email = UTF8ToString(emailPtr);
            var password = UTF8ToString(passwordPtr);
        
            // TODO(you): prompt the user to re-provide their sign-in credentials
            var credential = firebase.auth.EmailAuthProvider.credential(email, password);
            
            user.reauthenticateWithCredential(credential).then(() => {
                // User re-authenticated.
        
                var receiveUserClass = {
                    displayName: user.displayName,      // string
                    email: user.email,                  // string
                    uid: user.uid,                      // string
                };
                // serialize data1 to json
                var json = JSON.stringify(receiveUserClass);
                // calculate buffer size
                var bufferSize = lengthBytesUTF8(json) + 1;
                // allocate buffer memory
                var buffer = _malloc(bufferSize);
                // convert string to UTF8
                stringToUTF8(json, buffer, bufferSize);
                Module.dynCall_viii(callback, instanceID, buffer, true);
        
            }).catch((error) => {
              // An error occurred
              // calculate buffer size
              var errorStr = error.code.toString();
              var bufferSize = lengthBytesUTF8(errorStr) + 1;
              // allocate buffer memory
              var buffer = _malloc(bufferSize);
              // convert string to UTF8
              stringToUTF8(errorStr, buffer, bufferSize);
              // execute callback from c# defined
              Module.dynCall_viii(callback, instanceID, buffer, false);
            });
        }
        else
        {
            var errorStr = "current user is null";
            var bufferSize = lengthBytesUTF8(errorStr) + 1;
            // allocate buffer memory
            var buffer = _malloc(bufferSize);
            // convert string to UTF8
            stringToUTF8(errorStr, buffer, bufferSize);
            // execute callback from c# defined
            Module.dynCall_viii(callback, instanceID, buffer, false);
        }
    },
    
    FB_CreateReplayInfoStorageJS: function (instanceID, callback, uidPtr, replayPtr, index)
    {
        var uid = UTF8ToString(uidPtr);
        var replay = UTF8ToString(replayPtr);
        
        var storageRef = firebase.storage().ref();
        var replayRef = storageRef.child("user/" + uid + "/");
        var replayInfoRef = replayRef.child("replay_" + index + ".json");
        
        replayInfoRef.putString(replay).then((snapShot) => {
            Module.dynCall_viii(callback, instanceID, null, true);
        }).catch((error) => {
            // An error occurred
            // calculate buffer size
            var errorStr = error.code.toString();
            var bufferSize = lengthBytesUTF8(errorStr) + 1;
            // allocate buffer memory
            var buffer = _malloc(bufferSize);
            // convert string to UTF8
            stringToUTF8(errorStr, buffer, bufferSize);
            // execute callback from c# defined
            Module.dynCall_viii(callback, instanceID, buffer, false);
        });
    },

    FB_GetReplayInfoCountStorageJS: function (instanceID, callback, uidPtr)
    {
        var uid = UTF8ToString(uidPtr);

        var storageRef = firebase.storage().ref();
        var listRef = storageRef.child("user/" + uid);

        listRef.listAll()
            .then((res) => {
                var count = res.items.length;
                console.log("user info storage items = " + count);

                var result = res.items.filter(x => x.name != "profile.json");
                var buffer = null;
                if (result.length != 0) {
                    var array1 = new Array(result.length);
                    var i = 0;
                    var count = result.length;
                    result.forEach(itemRef => {
                        itemRef.getMetadata().then((metadata) => {
                            //console.log("timeCreated"+metadata.timeCreated);
                            var resStr = itemRef.name + " timeCreated:" + metadata.updated;
                            array1[i++] = resStr;
                            if (--count == 0) {
                                var json = JSON.stringify(array1);
                                var bufferSize = lengthBytesUTF8(json) + 1;
                                var buffer = _malloc(bufferSize);
                                console.log(json);
                                stringToUTF8(json, buffer, bufferSize);
                                Module.dynCall_viii(callback, instanceID, buffer, true);
                            }
                        }).catch((error) => {
                            //console.log(error.code.toString());
                            array1[i++] = itemRef.name;
                            if (--count == 0) {
                                var json = JSON.stringify(array1);
                                var bufferSize = lengthBytesUTF8(json) + 1;
                                var buffer = _malloc(bufferSize);
                                console.log(json);
                                stringToUTF8(json, buffer, bufferSize);
                                Module.dynCall_viii(callback, instanceID, buffer, true);
                            }
                        });
                    });
                }
                else
                {
                    Module.dynCall_viii(callback, instanceID, null, true);
                }
            }).catch((error) => {
                var errorStr = error.code.toString();
                var bufferSize = lengthBytesUTF8(errorStr) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(errorStr, buffer, bufferSize);
                Module.dynCall_viii(callback, instanceID, buffer, false);
            });
    },

    FB_GetReplayInfoStorageJS: function (instanceID, callback, uidPtr, index)
    {
        var uid = UTF8ToString(uidPtr);
        
        var storageRef = firebase.storage().ref();
        var replayRef = storageRef.child("user/" + uid + "/");
        var replayInfoRef = replayRef.child("replay_" + index + ".json");
        
        replayInfoRef.getDownloadURL()
            .then((url) => {
                // `url` is the download URL for xxxx
        
                // This can be downloaded directly:
                var xhr = new XMLHttpRequest();
                xhr.responseType = 'json';
                xhr.onload = (event) => {
                    var json = JSON.stringify(xhr.response);
                    //console.log("json=" + json);
        
                    // calculate buffer size
                    var bufferSize = lengthBytesUTF8(json) + 1;
                    // allocate buffer memory
                    var buffer = _malloc(bufferSize);
                    // convert string to UTF8
                    stringToUTF8(json, buffer, bufferSize);
                    Module.dynCall_viii(callback, instanceID, buffer, true);
                };
                xhr.open('GET', url);
                xhr.send();
            })
            .catch((error) => {
                // error
                // calculate buffer size
                var errorStr = error.code.toString();
                var bufferSize = lengthBytesUTF8(errorStr) + 1;
                // allocate buffer memory
                var buffer = _malloc(bufferSize);
                // convert string to UTF8
                stringToUTF8(errorStr, buffer, bufferSize);
                // execute callback from c# defined
                Module.dynCall_viii(callback, instanceID, buffer, false);
            });
    },
    
    FB_OnAuthStateChangedJS: function (instanceID, callback)
    {
        firebase.auth().onAuthStateChanged((user) => {
            if (user != null)
            {
                var receiveUserClass = {
                    displayName: user.displayName,
                    email: user.email,
                    uid: user.uid
                };
                var json = JSON.stringify(receiveUserClass);
                var bufferSize = lengthBytesUTF8(json) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(json, buffer, bufferSize);
                Module.dynCall_viii(callback, instanceID, buffer, true);
            }
            else
            {
                var errorStr = "current user is null";
                var bufferSize = lengthBytesUTF8(errorStr) + 1;
                // allocate buffer memory
                var buffer = _malloc(bufferSize);
                // convert string to UTF8
                stringToUTF8(errorStr, buffer, bufferSize);
                // execute callback from c# defined
                Module.dynCall_viii(callback, instanceID, buffer, false);
            }
        });
    },

    CheckOrientationJS: function (instanceID, callback)
    {
        function checkOrientation() {
        
            if (window.innerHeight > window.innerWidth) {
                Module.dynCall_viii(callback, instanceID, null, true);
            }
            else {
                Module.dynCall_viii(callback, instanceID, null, false);
            }
        }
        
        window.addEventListener("resize", checkOrientation);
        checkOrientation();
    },

}
mergeInto(LibraryManager.library, FirebasePlugin);
