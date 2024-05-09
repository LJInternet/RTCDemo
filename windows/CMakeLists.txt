
cmake_minimum_required(VERSION 3.10.2)

project("avmgrdemo")

set(CMAKE_CXX_STANDARD 11)

#add_definitions(-DWIN32_LEAN_AND_MEAN)
if (CMAKE_CL_64)
set(SDK_ROOT_PATH ${CMAKE_SOURCE_DIR}/rtc/win64)
else()
set(SDK_ROOT_PATH ${CMAKE_SOURCE_DIR}/rtc/win32)
endif()

message("xxxxxxxxxxxxx${SDK_ROOT_PATH}")
#add_subdirectory(${SDK_ROOT_PATH}/transfer/mediatransfer/src/main/jni ${SDK_ROOT_PATH}/out/mediatransfer)

include_directories( ./)
include_directories( ..)
include_directories( ${SDK_ROOT_PATH}/mediatransfer/header)
include_directories( ${SDK_ROOT_PATH}/rudp/header)

aux_source_directory(. DEMO_SRC)

find_library(LIB_RUDP rudp ${SDK_ROOT_PATH}/rudp)
find_library(LIB_BASESTONE basestone ${SDK_ROOT_PATH}/basestone)
find_library(LIB_UV uv ${SDK_ROOT_PATH}/libuv)
find_library(LIB_MEDIA_TRANSFER mediatransfer ${SDK_ROOT_PATH}/mediatransfer)
find_library(LIB_AUDIO_ENGINE audioengine ${SDK_ROOT_PATH}/audiocapture)

add_executable(RTCWinPush ./RTCWinPush.cc ./RtcTest.cpp)

target_link_libraries( RTCWinPush
	${LIB_MEDIA_TRANSFER}
    ${LIB_AUDIO_ENGINE}
    ${LIB_RUDP}
    ${LIB_UV}
    ${LIB_BASESTONE}
)

add_executable(RTCWinPull ./RTCWinPull.cc ./RtcTest.cpp)

target_link_libraries( RTCWinPull
	${LIB_MEDIA_TRANSFER}
    ${LIB_AUDIO_ENGINE}
    ${LIB_RUDP}
    ${LIB_UV}
    ${LIB_BASESTONE}
)

add_executable(RTCPushDemo ./RTCLinuxPushTest.cpp ./RtcTest.cpp)

target_link_libraries( RTCPushDemo
	${LIB_MEDIA_TRANSFER}
    ${LIB_AUDIO_ENGINE}
    ${LIB_RUDP}
    ${LIB_UV}
    ${LIB_BASESTONE}
)

add_executable(RTCPullDemo ./RTCLinuxPullTest.cpp ./RtcTest.cpp)

target_link_libraries( RTCPullDemo
	${LIB_MEDIA_TRANSFER}
    ${LIB_AUDIO_ENGINE}
    ${LIB_RUDP}
    ${LIB_UV}
    ${LIB_BASESTONE}
)

add_executable(P2PRTMTest ./P2PRTMTest.cpp)

target_link_libraries( P2PRTMTest
    ${LIB_RUDP}
    ${LIB_UV}
    ${LIB_BASESTONE}
)

add_executable(MultiRTMTest ./MultiRTMTest.cpp)

target_link_libraries( MultiRTMTest
    ${LIB_RUDP}
    ${LIB_UV}
    ${LIB_BASESTONE}
)