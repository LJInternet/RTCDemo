CMAKE_MINIMUM_REQUIRED(VERSION 3.10)

PROJECT(RtcLinuxDemo LANGUAGES CXX)
option(GROUP_BY_EXPLORER ON)

SET(CMAKE_CXX_STANDARD 17)

SET(CMAKE_BUILD_TYPE Debug)
SET(CMAKE_EXPORT_COMPILE_COMMANDS ON)

MESSAGE(INFO "--------------------------")
MESSAGE(STATUS "CMAKE_INSTALL_PREFIX :${CMAKE_INSTALL_PREFIX}")
IF (NOT CMAKE_TOOLCHAIN_FILE)
    MESSAGE(STATUS "CMAKE_TOOLCHAIN_FILE:  ${CMAKE_TOOLCHAIN_FILE}")
ENDIF()


# build type
IF (NOT CMAKE_BUILD_TYPE)
    SET(CMAKE_BUILD_TYPE "Debug" CACHE STRING "Choose Release or Debug" FORCE)
ENDIF()
IF (${CMAKE_BUILD_TYPE} STREQUAL "Debug")
    add_definitions("-g")
ENDIF()

MESSAGE(STATUS "Build Type: ${CMAKE_BUILD_TYPE}")

SET(LIBS_DIR ${PROJECT_SOURCE_DIR}/libs)

SET(LJSDK_MEDIATRANSFER "${LIBS_DIR}/mediatransfer")
SET(LJSDK_BASESTONE "${LIBS_DIR}/basestone")
SET(LJSDK_RUDP "${LIBS_DIR}/rudp")
SET(LJSDK_AUDIOENGINE "${LIBS_DIR}/audiocapture")
SET(LJSDK_LINUX_X64 "${LIBS_DIR}/linux-x64")
SET(LJSDK_INCLUDE_DIR "${LIBS_DIR}/mediatransfer/header")
SET(LJSDK_LIBS basestone mediatransfer rudp audioengine)

MESSAGE(STATUS "include: ${LJSDK_INCLUDE_DIR}")

include_directories(
    src
    ${LJSDK_INCLUDE_DIR}
)

link_directories(
	${LJSDK_MEDIATRANSFER}
	${LJSDK_BASESTONE}
	${LJSDK_RUDP}
	${LJSDK_AUDIOENGINE}
	${LJSDK_LINUX_X64}
)

file (GLOB SRC "${CMAKE_SOURCE_DIR}/src/*.cpp")


add_executable(RtcLinuxDemo "${CMAKE_SOURCE_DIR}/src/main.cpp")
target_link_libraries(RtcLinuxDemo
        ${LJSDK_LIBS}
        libpthread.so)
		

SET(LJSDK_RUDP_INCLUDE_DIR "${LIBS_DIR}/rudp/header")
include_directories(${LJSDK_RUDP_INCLUDE_DIR})
SET(LJSDK_RUDP_TEST basestone rudp)	
add_executable(P2PRTMTest "${CMAKE_SOURCE_DIR}/src/P2PRTMTest.cpp")
target_link_libraries(P2PRTMTest
        ${LJSDK_RUDP_TEST}
        libpthread.so)
		
add_executable(MultiRTMTest "${CMAKE_SOURCE_DIR}/src/MultiRTMTest.cpp")
target_link_libraries(MultiRTMTest
        ${LJSDK_RUDP_TEST}
        libpthread.so)
