#pragma once

#include <stdint.h>
#include <string>
#include "common_defs.h"

namespace LJ
{

/**
 * DateTime is a class representing date of GMT.
 */
class BASESTONE_EXTERN DateTime
{
public:
    /**
     * Get a DateTime object for current date.
     * @return DateTime object for current date.
     */
    static DateTime now();
    

    static uint64_t currentTimeMillis();

    /**
     * Constructor for a specific date.
     */
    DateTime(unsigned int year,
         unsigned int month,
         unsigned int day,
         unsigned int hour,
         unsigned int minute,
         unsigned int second,
         unsigned int millisecond);

    /**
     * Copy constructor.
     */
    DateTime(const DateTime &date);

    /**
     * Constructor from a milliseconds from 1970, Jan. 1st.
     */
    DateTime(uint64_t unixTimestampMillisec);

    /**
     * Return year of date.
     * @return year of date.
     */
    unsigned int year() const;

    /**
     * Return month of date.
     * @return month of date.
     */
    unsigned int month() const;

    /**
     * Return day of date.
     * @return day of date.
     */
    unsigned int day() const;

    /**
     * Return hour of date.
     * @return hour of date.
     */
    unsigned int hour() const;

    /**
     * Return minute of date.
     * @return minute of date.
     */
    unsigned int minute() const;

    /**
     * Return second of date.
     * @return second of date.
     */
    unsigned int second() const;

    /**
     * Return millisecond of date.
     * @return millisecond of date.
     */
    unsigned int millisecond() const;

    /**
     * Return milliseconds from 1970-01-01.
     * @return milliseconds from 1970-01-01.
     */
    uint64_t unixTimestampMillisec() const;

    /**
     * Add time to date.
     * @param millisecond time to be added in milliseconds.
     * @return the new DateTime object.
     */
    DateTime operator+(int millisecond) const;

    /**
     * Subtract a DateTime object.
     * @param date DateTime object to be subtracted.
     * @return delta date in milliseconds.
     */
    int64_t operator-(const DateTime &date) const;

    /**
     * Compare if the current date is equals to another DateTime object.
     * @param date the DateTime object to be compared.
     * @return true if date is matched.
     */
    bool operator==(const DateTime &date) const;

    /**
     * Compare to DateTime object to see if current date is less than another.
     * @param date the DateTime object to be compared.
     * @return true if current date is lesser.
     */
    bool operator<(const DateTime &date) const;

    /**
     * Get date string in long format (YYYY-MM-DD hh:mm:ss.mmm)
     * @param timezone time zone of date string
     * @return date string in format
     */
    std::string toString(short timezone) const;

    /**
     * Get date string in long format (YYYY-MM-DD hh:mm:ss)
     * @param timezone time zone of date string
     * @return date string in format
     */
    std::string toStringSecond(short timezone) const;

    /**
     * Get date string in short format (MM-DD hh:mm:ss.mmm)
     * @param timezone time zone of date string
     * @return date string in format
     */
    std::string toShortString(short timezone) const;

    /**
     * Get current time zone offset from GMT.
     * @return time zone offset from GMT.
     */
    static int currentTimeZone();

private:
    void calculateUnixTimestampMillisec();
    void calculateDateTime();

    unsigned int _year;
    unsigned int _month;
    unsigned int _day;
    unsigned int _hour;
    unsigned int _minute;
    unsigned int _second;
    unsigned int _millisecond;
    uint64_t _unixTimestampMillisec;

};

}
