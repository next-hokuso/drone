var TestPlugin = {
    // IndexDB�̃p�����[�^��ێ�����I�u�W�F�N�g�A�_�b�V���{�[�h�Ŏ擾�����l��]�L���Ă��������B
    $FirebaseConfig: {
        apiKey: "AIzaSyA3YeUG0tpstsPBMdkulk-IcbDyoYV0H_E",
        authDomain: "drone-star-training-dev.firebaseapp.com",
        projectId: "drone-star-training-dev",
        storageBucket: "drone-star-training-dev.appspot.com",
        messagingSenderId: "128999627223",
        appId: "1:128999627223:web:b4134357540a3e58a35392",
        measurementId: "G-L4HTJJKDBR"
    },
    $DB: null
    ,
    FirebaseInit: function () {
        window.firebase.initializeApp(FirebaseConfig); // window��t�^���邱�ƂŃO���[�o���Ȓl���擾���邱�Ƃ��ł��܂��B
        DB = window.firebase.firestore();
    },
    ReadFirestoreJS: function (instanceID, callback) {
        // instanceID�͖{���\�b�h���Ăяo�����C���X�^���X��ID
        // callback��C#���Œ�`�����R�[���o�b�N�֐�
        
        // �h�L�������g�̎擾
        var docRef = DB.collection("FirestoreCollection").doc("FirestoreDocument");

        docRef.get().then(function (doc) {
            if (doc.exists) {
                
                var json = JSON.stringify(doc.data()); // json�𕶎���ɕϊ����ă|�C���^�ɓn��
                var bufferSize = lengthBytesUTF8(json) + 1; // �o�b�t�@����T�C�Y���擾
                var buffer = _malloc(bufferSize); // �o�b�t�@�p���������m��
                stringToUTF8(json, buffer, bufferSize); // �������UTF8�ɕϊ����ă|�C���^�ɓn��
                Module.dynCall_vii(callback, instanceID, buffer); // C#�Œ�`�����R�[���o�b�N�֐����Ăяo��
            } else {
                console.log("�h�L�������g��������܂���");
            }
        }).catch(function (error) {
            console.log("Error:", error);
        });
    },
    WriteFirestoreJS: function (keyPtr, valuePtr) {
        // ������̓|�C���^�Ƃ��ēn�����̂ŕ�����ɕϊ�����
        var key = UTF8ToString(keyPtr);
        var value = UTF8ToString(valuePtr);

        // Firestore�h�L�������g�̎擾
        var docRef = DB.collection("FirestoreCollection").doc("FirestoreDocument");
        
        docRef.update({
            [key]: value
        })
        .then(function () {
            console.log("�h�L�������g�̍X�V�ɐ������܂���");
        })
        .catch(function (error) {
            console.error("Error: ", error);
        });
    },
    LoginJS: function (){
        //������
        var email = document.getElementById('email').value;
        var password = "ai4976ai";

        firebase.auth().signInWithEmailAndPassword(email, password)
          .then((userCredential) => {
            // Signed in
            var user = userCredential.user;
            // ...
            console.log("���O�C���ɐ������܂���");
          })
          .catch((error) => {
            var errorCode = error.code;
            var errorMessage = error.message;
            console.error("Error: ", error);
          });




    //const email = "suzuki@a-i.co.jp";
    //const password = "ai4976ai";
    //firebase.auth().signInWithEmailAndPassword(email, password)
    //    .then((userCredential) => {
    //        //���O�C�����̏���
    //        window.alert("Login Success");
    //    })
    //    .catch((error) => {
    //        //console.log(error.code);
    //        window.alert(error.code);
    //    });

    },
};
autoAddDeps(FirebasePlugin, '$DB'); // $DB�̃R�[�h�X�g���b�v�h�~
autoAddDeps(FirebasePlugin, '$FirebaseConfig'); // $FirebaseConfig�̃R�[�h�X�g���b�v�h�~
mergeInto(LibraryManager.library, TestPlugin); // LibraryManager��FirebasePlugin�𓝍�
