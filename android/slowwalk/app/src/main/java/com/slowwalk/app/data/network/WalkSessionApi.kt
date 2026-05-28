package com.slowwalk.app.data.network

import com.slowwalk.app.sync.WalkSessionRequest
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.POST

interface WalkSessionApi {

    @POST("api/walk-sessions")
    suspend fun postSession(@Body request: WalkSessionRequest): Response<Unit>
}
