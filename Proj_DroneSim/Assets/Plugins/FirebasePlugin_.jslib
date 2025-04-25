var TestPlugin = {
    // IndexDBのパラメータを保持するオブジェクト、ダッシュボードで取得した値を転記してください。
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
        window.firebase.initializeApp(FirebaseConfig); // windowを付与することでグローバルな値を取得することができます。
        DB = window.firebase.firestore();
    },
    ReadFirestoreJS: function (instanceID, callback) {
        // instanceIDは本メソッドを呼び出したインスタンスのID
        // callbackはC#側で定義したコールバック関数
        
        // ドキュメントの取得
        var docRef = DB.collection("FirestoreCollection").doc("FirestoreDocument");

        docRef.get().then(function (doc) {
            if (doc.exists) {
                
                var json = JSON.stringify(doc.data()); // jsonを文字列に変換してポインタに渡す
                var bufferSize = lengthBytesUTF8(json) + 1; // バッファするサイズを取得
                var buffer = _malloc(bufferSize); // バッファ用メモリを確保
                stringToUTF8(json, buffer, bufferSize); // 文字列をUTF8に変換してポインタに渡す
                Module.dynCall_vii(callback, instanceID, buffer); // C#で定義したコールバック関数を呼び出す
            } else {
                console.log("ドキュメントが見つかりません");
            }
        }).catch(function (error) {
            console.log("Error:", error);
        });
    },
    WriteFirestoreJS: function (keyPtr, valuePtr) {
        // 文字列はポインタとして渡されるので文字列に変換する
        var key = UTF8ToString(keyPtr);
        var value = UTF8ToString(valuePtr);

        // Firestoreドキュメントの取得
        var docRef = DB.collection("FirestoreCollection").doc("FirestoreDocument");
        
        docRef.update({
            [key]: value
        })
        .then(function () {
            console.log("ドキュメントの更新に成功しました");
        })
        .catch(function (error) {
            console.error("Error: ", error);
        });
    },
    LoginJS: function (){
        //第一引数
        var email = document.getElementById('email').value;
        var password = "ai4976ai";

        firebase.auth().signInWithEmailAndPassword(email, password)
          .then((userCredential) => {
            // Signed in
            var user = userCredential.user;
            // ...
            console.log("ログインに成功しました");
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
    //        //ログイン時の処理
    //        window.alert("Login Success");
    //    })
    //    .catch((error) => {
    //        //console.log(error.code);
    //        window.alert(error.code);
    //    });

    },
};
autoAddDeps(FirebasePlugin, '$DB'); // $DBのコードストリップ防止
autoAddDeps(FirebasePlugin, '$FirebaseConfig'); // $FirebaseConfigのコードストリップ防止
mergeInto(LibraryManager.library, TestPlugin); // LibraryManagerにFirebasePluginを統合
