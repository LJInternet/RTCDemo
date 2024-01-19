#pragma once
#include "LJIntTypes.h"
#include "LJBlockbuffer.h"
#include "LJVarstr.h"
#include <string>
#include <iostream>
#include <stdexcept>
#include <map>
#include <vector>
#include <deque>
#include <set>
#include <string.h>
#include "log.h"
namespace LJMediaLibrary {
#define SAFE_UNMARSHAL_REQUEST(mie, up) \
    mie.unmarshal(up); \
    if (up.isUnpackError()) \
    { \
        LJ::LOG("LJ_RTC", LJ::LOG_ERROR, "%s in func %s, type %u", "unmarshal ERROR", __FUNCTION__); \
        return 0; \
    }

#define SAFE_UNMARSHAL_NOTYPE_REQUEST(mie, up) \
    mie.unmarshal(up); \
    if (up.isUnpackError()) \
    { \
        LJ::LOG("LJ_RTC", LJ::LOG_ERROR, "%s in func %s, type %u", "unmarshal ERROR", __FUNCTION__); \
        return 0; \
    }
}
namespace ljtransfer
{

namespace mediaSox
{

class PackBuffer
{
public:
	char* data()
	{
		return bb.data();
	}

	size_t size() const
	{
		return bb.size();
	}

	bool resize(size_t n)
	{
		return bb.resize(n);
	}

	bool append(const char* data, size_t size)
	{
		return bb.append(data, size);
	}

	bool append(const char* data)
	{
		return append(data, ::strlen(data));
	}

	bool replace(size_t pos, const char* rep, size_t n)
	{
		return bb.replace(pos, rep, n);
	}

	bool reserve(size_t n)
	{
		return bb.reserve(n);
	}

private:
	// use big-block. more BIG? MAX 64K*4k = 256M
	typedef BlockBuffer<def_block_alloc_4k, 65536> BB;
	BB bb;
};



#define XHTONS
#define XHTONL
#define XHTONLL

#define XNTOHS XHTONS
#define XNTOHL XHTONL
#define XNTOHLL XHTONLL

class Pack
{
private:
	Pack (const Pack& o);
	Pack& operator = (const Pack& o);

public:
	uint16_t xhtons(uint16_t i16)
	{
		return XHTONS(i16);
	}

	uint32_t xhtonl(uint32_t i32)
	{
		return XHTONL(i32);
	}

	uint64_t xhtonll(uint64_t i64)
	{
		return XHTONLL(i64);
	}

	// IMPORTANT remember the buffer-size before pack. see data(), size()
	// reserve a space to replace packet header after pack parameter
	// sample below: OffPack. see data(), size()
	Pack(PackBuffer& pb, size_t off = 0)
	: m_buffer(pb)
	, m_bPackError(false)
	{
		m_offset = pb.size() + off;
		if (!m_buffer.resize(m_offset))
		{
			m_bPackError = true;
		}
	}

	// access this packet.
	char* data()
	{
		return m_buffer.data() + m_offset;
	}

	const char* data() const
	{
		return m_buffer.data() + m_offset;
	}

	size_t size() const
	{
		return m_buffer.size() - m_offset;
	}

	Pack& push(const void* s, size_t n)
	{
		if (!m_buffer.append((const char *)s, n))
		{
			m_bPackError = true;
		}
		return *this;
	}

	Pack& push(const void* s)
	{
		if (!m_buffer.append((const char *)s))
		{
			m_bPackError = true;
		}
		return *this;
	}

	Pack& push_uint8(uint8_t u8)
	{
		return push(&u8, 1);
	}

	Pack& push_uint16(uint16_t u16)
	{
		u16 = xhtons(u16);
		return push(&u16, 2);
	}

	Pack& push_uint32(uint32_t u32)
	{
		u32 = xhtonl(u32);
		return push(&u32, 4);
	}

	Pack& push_uint64(uint64_t u64)
	{
		u64 = xhtonll(u64);
		return push(&u64, 8);
	}

	Pack& push_varstr(const Varstr& vs)
	{
		return push_varstr(vs.data(), vs.size());
	}

	Pack& push_varstr(const void* s)
	{
		return push_varstr(s, strlen((const char *)s));
	}

	Pack& push_varstr(const std::string& s)
	{
		return push_varstr(s.data(), s.size());
	}

	Pack& push_varstr(const void* s, size_t len)
	{
		if (len > 0xFFFF)
		{
			m_bPackError = true;
			len = 0;
		}
		return push_uint16(uint16_t(len)).push(s, len);
	}

	Pack& push_varstr32(const void* s, size_t len)
	{
		if (len > 0xFFFFFFFF) 
		{
			m_bPackError = true;
			len = 0;
		}
		return push_uint32(uint32_t(len)).push(s, len);
	}

	virtual ~Pack() {}

public:
	// replace. pos is the buffer offset, not this Pack m_offset
	size_t replace(size_t pos, const void* data, size_t rplen)
	{
		if (!m_buffer.replace(pos, (const char*)data, rplen))
		{
			m_bPackError = true;
		}
		return pos + rplen;
	}

	size_t replace_uint8(size_t pos, uint8_t u8)
	{
		return replace(pos, &u8, 1);
	}

	size_t replace_uint16(size_t pos, uint16_t u16)
	{
		u16 = xhtons(u16);
		return replace(pos, &u16, 2);
	}

	size_t replace_uint32(size_t pos, uint32_t u32)
	{
		u32 = xhtonl(u32);
		return replace(pos, &u32, 4);
	}

	bool isPackError() const
	{
		return m_bPackError;
	}

	void setPackError(bool val)
	{
		m_bPackError = val;
	}

protected:
	PackBuffer& m_buffer;
	size_t m_offset;
	bool m_bPackError;
};

class Unpack
{
public:
	Unpack(const void* data, size_t size)
	: m_bUnpackError(false)
	{
		reset(data, size);
	}

	virtual ~Unpack()
	{
		m_data = NULL;
	}

public:
	uint16_t xntohs(uint16_t i16) const
	{
		return XNTOHS(i16);
	}

	uint32_t xntohl(uint32_t i32) const
	{
		return XNTOHL(i32);
	}

	uint64_t xntohll(uint64_t i64) const
	{
		return XNTOHLL(i64);
	}

	void reset(const void* data, size_t size) const
	{
		m_data = (const char *)data;
		m_size = size;
	}

	operator const void *() const
	{
		return m_data;
	}

	bool operator!() const
	{
		return (NULL == m_data);
	}

	std::string pop_varstr() const
	{
		Varstr vs = pop_varstr_ptr();
		return std::string(vs.data(), vs.size());
	}

	std::string pop_varstr32() const
	{
		Varstr vs = pop_varstr32_ptr();
		return std::string(vs.data(), vs.size());
	}

	std::string pop_fetch(size_t k) const
	{
		return std::string(pop_fetch_ptr(k), k);
	}

	void finish() const
	{
		if (!empty()) 
		{
			m_bUnpackError = true;
		}
	}

	uint8_t pop_uint8() const
	{
		if (m_size < 1u)
		{
			m_bUnpackError = true;
			return 0;
		}

		uint8_t i8 = *((uint8_t*)m_data);
		m_data += 1u;
		m_size -= 1u;
		return i8;
	}

	uint16_t pop_uint16() const
	{
		if (m_size < 2u)
		{
			m_bUnpackError = true;
			return 0;
		}

		uint16_t i16 = 0;
		memcpy(&i16, m_data, sizeof(uint16_t));
		i16 = xntohs(i16);

		m_data += 2u;
		m_size -= 2u;
		return i16;
	}

	uint32_t pop_uint32() const
	{
		if (m_size < 4u)
		{
			m_bUnpackError = true;
			return 0;
		}

		uint32_t i32 = 0;
		memcpy(&i32, m_data, sizeof(uint32_t));
		i32 = xntohl(i32);
		m_data += 4u;
		m_size -= 4u;
		return i32;
	}

	uint32_t peek_uint32() const
	{
		if (m_size < 4u)
		{
			m_bUnpackError = true;
			return 0;
		}

		uint32_t i32 = 0;
		memcpy(&i32, m_data, sizeof(uint32_t));
		i32 = xntohl(i32);
		return i32;
	}

	uint64_t pop_uint64() const
	{
		if (m_size < 8u)
		{
			m_bUnpackError = true;
			return 0;
		}

		uint64_t i64 = 0;
		memcpy(&i64, m_data, sizeof(uint64_t));
		i64 = xntohll(i64);
		m_data += 8u;
		m_size -= 8u;
		return i64;
	}

	Varstr pop_varstr_ptr() const
	{
		Varstr vs;
		vs.m_size = pop_uint16();
		vs.m_data = pop_fetch_ptr(vs.m_size);
		return vs;
	}

	Varstr pop_varstr32_ptr() const
	{
		Varstr vs;
		vs.m_size = pop_uint32();
		vs.m_data = pop_fetch_ptr(vs.m_size);
		return vs;
	}

	const char* pop_fetch_ptr(size_t& k) const
	{
		if (m_size < k)
		{
			m_bUnpackError = true;
			k = m_size;
		}

		const char* p = m_data;
		m_data += k;
		m_size -= k;
		return p;
	}

	bool empty() const
	{
		return m_size == 0;
	}

	const char* data() const
	{
		return m_data;
	}

	size_t size() const
	{
		return m_size;
	}

	bool isUnpackError() const
	{
		return m_bUnpackError;
	}

private:
	mutable const char* m_data;
	mutable size_t m_size;
	mutable bool m_bUnpackError;
};

struct Marshallable
{
	virtual void marshal(Pack &) const = 0;
	virtual void unmarshal(const Unpack &) = 0;
	virtual ~Marshallable() {}
	virtual std::ostream& trace(std::ostream& os) const
	{
		return os << "trace Marshallable [ not immplement ]";
	}
};

inline std::ostream& operator << (std::ostream& os, const Marshallable& m)
{
	return m.trace(os);
}

inline Pack& operator << (Pack& p, const Marshallable& m)
{
	m.marshal(p);
	return p;
}

inline const Unpack& operator >> (const Unpack& p, const Marshallable& m)
{
	const_cast<Marshallable &>(m).unmarshal(p);
	return p;
}

template <class T1, class T2>
inline Pack& operator << (Pack& p, const std::map<T1, T2>& map)
{
	marshal_container(p, map);
	return p;
}

template <class T1, class T2>
inline const mediaSox::Unpack& operator >> (const mediaSox::Unpack& p, std::map<T1, T2>& map)
{
	unmarshal_container(p, std::inserter(map, map.begin()));
	return p;
}

struct Voidmable
	: public mediaSox::Marshallable
{
	virtual void marshal(Pack &) const {}
	virtual void unmarshal(const Unpack &) {}
};

struct Mulmable
	: public mediaSox::Marshallable
{
	Mulmable(const mediaSox::Marshallable& m1, const mediaSox::Marshallable& m2)
	: mm1(m1)
	, mm2(m2)
	{
	}

	const mediaSox::Marshallable& mm1;
	const mediaSox::Marshallable& mm2;

	virtual void marshal(Pack &p) const
	{
		p << mm1 << mm2;
	}

	virtual void unmarshal(const mediaSox::Unpack &p)
	{
		(void)p;assert(false);
	}

	virtual std::ostream& trace(std::ostream& os) const
	{
		return os << mm1 << mm2;
	}
};

struct Mulumable : public mediaSox::Marshallable
{
	Mulumable(mediaSox::Marshallable& m1, mediaSox::Marshallable& m2)
	: mm1(m1)
	, mm2(m2)
	{
	}

	mediaSox::Marshallable& mm1;
	mediaSox::Marshallable& mm2;

	virtual void marshal(Pack &p) const
	{
		p << mm1 << mm2;
	}

	virtual void unmarshal(const mediaSox::Unpack &p)
	{
		p >> mm1 >> mm2;
	}

	virtual std::ostream& trace(std::ostream& os) const
	{
		return os << mm1 << mm2;
	}
};

struct Rawmable
	: public mediaSox::Marshallable
{
	Rawmable(const char* data, size_t size)
	: m_data(data)
	, m_size(size)
	{
	}

	template < class T >
	explicit Rawmable(T& t )
	: m_data(t.data())
	, m_size(t.size())
	{
	}

	const char* m_data;
	size_t m_size;

	virtual void marshal(mediaSox::Pack& p) const
	{
		p.push(m_data, m_size);
	}

	virtual void unmarshal(const mediaSox::Unpack &)
	{
		assert(false);
	}
};

inline Pack& operator << (Pack& p, bool sign)
{
	p.push_uint8(sign ? 1 : 0);
	return p;
}

inline Pack& operator << (Pack& p, uint8_t i8)
{
	p.push_uint8(i8);
	return p;
}

inline Pack& operator << (Pack& p, uint16_t i16)
{
	p.push_uint16(i16);
	return p;
}

inline Pack& operator << (Pack& p, uint32_t i32)
{
	p.push_uint32(i32);
	return p;
}
inline Pack& operator << (Pack& p, uint64_t i64)
{
	p.push_uint64(i64);
	return p;
}

inline Pack& operator << (Pack& p, const std::string& str)
{
	p.push_varstr(str);
	return p;
}

inline Pack& operator << (Pack& p, const Varstr& pstr)
{
	p.push_varstr(pstr);
	return p;
}

inline const Unpack& operator >> (const Unpack& p, Varstr& pstr)
{
	pstr = p.pop_varstr_ptr();
	return p;
}

inline const Unpack& operator >> (const Unpack& p, uint32_t& i32)
{
	i32 = p.pop_uint32();
	return p;
}

inline const Unpack& operator >> (const Unpack& p, uint64_t& i64)
{
	i64 = p.pop_uint64();
	return p;
}

inline const Unpack& operator >> (const Unpack& p, std::string& str)
{
	str = p.pop_varstr();
	return p;
}

inline const Unpack& operator >> (const Unpack& p, uint16_t& i16)
{
	i16 = p.pop_uint16();
	return p;
}

inline const Unpack& operator >> (const Unpack& p, uint8_t& i8)
{
	i8 = p.pop_uint8();
	return p;
}

inline const Unpack& operator >> (const Unpack& p, bool& sign)
{
	sign = (p.pop_uint8() == 0) ? false : true;
	return p;
}

template <class T1, class T2>
inline std::ostream& operator << (std::ostream& s, const std::pair<T1, T2>& p)
{
	s << p.first << '=' << p.second;
	return s;
}

template <class T1, class T2>
inline Pack& operator << (Pack& s, const std::pair<T1, T2>& p)
{
	s << p.first << p.second;
	return s;
}

template <class T1, class T2>
inline const Unpack& operator >> (const mediaSox::Unpack& s, std::pair<const T1, T2>& p)
{
	const T1& m = p.first;
	T1& m2 = const_cast<T1 &>(m);
	s >> m2 >> p.second;
	return s;
}

template <class T1, class T2>
inline const Unpack& operator >> (const mediaSox::Unpack& s, std::pair<T1, T2>& p)
{
	s >> p.first >> p.second;
	return s;
}

template <class T>
inline Pack& operator << (Pack& p, const std::vector<T>& vec)
{
	marshal_container(p, vec);
	return p;
}

template <class T>
inline const Unpack& operator >> (const mediaSox::Unpack& p, std::vector<T>& vec)
{
	unmarshal_container(p, std::back_inserter(vec)); 
	return p;
}

template <class T>
inline Pack& operator << (Pack& p, const std::deque<T>& deq)
{
	marshal_container(p, deq);
	return p;
}

template <class T>
inline const Unpack& operator >> (const mediaSox::Unpack& p, std::deque<T>& deq)
{
	unmarshal_container(p, std::back_inserter(deq)); 
	return p;
}

template <class T>
inline Pack& operator << (Pack& p, const std::set<T>& set)
{
	marshal_container(p, set);
	return p;
}

template <class T>
inline const Unpack& operator >> (const mediaSox::Unpack& p, std::set<T>& set)
{
	unmarshal_container(p, std::inserter(set, set.begin()));
	return p;
}

template < typename ContainerClass >
inline void marshal_container(Pack& p, const ContainerClass& c)
{
	p.push_uint32(uint32_t(c.size())); // use uint32 ...
	for (typename ContainerClass::const_iterator i = c.begin(); i != c.end(); ++i)
		p << *i;
}

template < typename OutputIterator >
inline void unmarshal_container(const Unpack& p, OutputIterator i)
{
	for (uint32_t count = p.pop_uint32(); count > 0; --count)
	{
		typename OutputIterator::container_type::value_type tmp;
		p >> tmp;
		*i = tmp;
		++i;
		if(p.isUnpackError())
		{
			break;
		}
	}
}

template < typename OutputIterator >
inline void unmarshal_container_pair(const Unpack &p, OutputIterator i )
{
	for (uint32_t count = p.pop_uint32(); count > 0; --count)
	{
		typename OutputIterator::container_type::value_type::second_type d;
		typename OutputIterator::container_type::value_type tmp(0,d);
		p >> tmp;
		*i = tmp;
		++i;
		if(p.isUnpackError())
		{
			break;
		}
	}
}

template < typename OutputContainer>
inline void unmarshal_containerEx(const Unpack& p, OutputContainer& c)
{
	for(uint32_t count = p.pop_uint32() ; count >0 ; --count)
	{
		typename OutputContainer::value_type tmp;
		tmp.unmarshal(p);
		c.push_back(tmp);
		if(p.isUnpackError())
		{
			break;
		}
	}
}

template < typename ContainerClass >
inline std::ostream& trace_container(std::ostream& os, const ContainerClass& c, char div='\n')
{
	for (typename ContainerClass::const_iterator i = c.begin(); i != c.end(); ++i)
		os << *i << div;
	return os;
}

inline bool PacketToString(const mediaSox::Marshallable &objIn, std::string &strOut)
{
	PackBuffer buffer;
	Pack pack(buffer);
	objIn.marshal(pack);                  							// ������
	strOut.assign(pack.data(), pack.size());

	return (pack.isPackError() == false);
}

inline bool PacketToString(const mediaSox::Marshallable &objIn, uint32_t uri, std::string &strOut)
{
	PackBuffer buffer;
	Pack pack(buffer);
	pack.push_uint32(0);               								// ����С�����ڻ���֪��
	pack.push_uint32(uri);          								// URI
	pack.push_uint16(200);              							// �̶�ֵ��д��200
	objIn.marshal(pack);                  							// ������
	pack.replace_uint32(0, (uint32_t)pack.size());					// ������д���Ĵ�С
	strOut.assign(pack.data(), pack.size());

	return (pack.isPackError() == false);
}

} // namespace mediaSox

} // namespace ljtransfer
