// Tencent is pleased to support the open source community by making Mars available.
// Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.

// Licensed under the MIT License (the "License"); you may not use this file except in 
// compliance with the License. You may obtain a copy of the License at
// http://opensource.org/licenses/MIT

// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" basis, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific language governing permissions and
// limitations under the License.


/*
 * compiler_util.h
 *
 *  Created on: 2013-11-15
 *      Author: yerungui
 */

#ifndef COMM_COMPILER_UTIL_H_
#define COMM_COMPILER_UTIL_H_

#if defined(__GNUC__)
#define WEAK_FUNC     __attribute__((weak))
#elif defined(_MSC_VER) && !defined(_LIB)
#define WEAK_FUNC __declspec(selectany)
#else
#define WEAK_FUNC
#endif

#if defined(__GNUC__)
#define EXPORT_FUNC __attribute__ ((visibility ("default")))
#elif defined(_MSC_VER)
#define EXPORT_FUNC __declspec(dllexport)
#else
#error "export"
#endif

#ifdef _WIN32
/* Windows - set up dll import/export decorators. */
# if defined(BUILDING_XLOG_SHARED)
    /* Building shared library. */
#   define XLOG_EXTERN __declspec(dllexport)
# elif defined(USING_XLOG_SHARED)
    /* Using shared library. */
#   define XLOG_EXTERN __declspec(dllimport)
# else
    /* Building static library. */
#   define XLOG_EXTERN /* nothing */
# endif
#elif __GNUC__ >= 4
# define XLOG_EXTERN __attribute__((visibility("default")))
#elif defined(__SUNPRO_C) && (__SUNPRO_C >= 0x550) /* Sun Studio >= 8 */
# define XLOG_EXTERN __global
#else
# define XLOG_EXTERN /* nothing */
#endif

#ifndef VARIABLE_IS_NOT_USED
#ifdef __GNUC__
#define VARIABLE_IS_NOT_USED __attribute__ ((unused))
#else
#define VARIABLE_IS_NOT_USED
#endif
#endif

#endif /* COMM_COMPILER_UTIL_H_ */
