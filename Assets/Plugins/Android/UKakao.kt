package com.Sailors.ColorWar


import android.content.ContentValues.TAG
import android.util.Log
import com.kakao.sdk.auth.model.OAuthToken
import com.kakao.sdk.user.UserApiClient
import com.unity3d.player.UnityPlayer

class UKakao {
    fun KakaoLogin() {
        // 로그인 공통 callback 구성
        val callback: (OAuthToken?, Throwable?) -> Unit = { token, error ->
            if (error != null) {
                Log.e(TAG, "로그인 실패", error)
                println("로그인 실패 : $error")
            }
            else if (token != null) {
                Log.i(TAG, "로그인 성공 ${token.accessToken}")
                println("로그인 성공 ${token.accessToken}")

                requestUserInfo()
            }
        }

// 카카오톡이 설치되어 있으면 카카오톡으로 로그인, 아니면 카카오계정으로 로그인
        if (UserApiClient.instance.isKakaoTalkLoginAvailable(UnityPlayer.currentActivity)) {
            UserApiClient.instance.loginWithKakaoTalk(UnityPlayer.currentActivity, callback = callback)
        } else {
            UserApiClient.instance.loginWithKakaoAccount(UnityPlayer.currentActivity, callback = callback)
        }
    }

    private fun requestUserInfo() {
        // Request user information using the obtained access token
        UserApiClient.instance.me { user, error ->
            if (error != null) {
                Log.e(TAG, "사용자 정보 요청 실패", error)
                println("사용자 정보 요청 실패 : $error")
            } else if (user != null) {
                // The user ID is available in the 'id' field of the user object
                val kakaoUserId = user.id
                Log.i(TAG, "KakaoTalk 사용자 ID: $kakaoUserId")
                println("KakaoTalk 사용자 ID: $kakaoUserId")

                // Send user ID to backend and handle the response
                UnityPlayer.UnitySendMessage("LoginManager", "OnResult", kakaoUserId.toString());
            }
        }
    }

}
