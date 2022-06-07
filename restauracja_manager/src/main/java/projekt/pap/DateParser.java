package projekt.pap;

import java.lang.IllegalArgumentException;
import java.sql.Timestamp;
import java.time.LocalDate;
import java.time.LocalDateTime;

public class DateParser {
    public static String parseNow(){
        String result = null;

        var now = LocalDateTime.now();

        var year = String.valueOf(now.getYear());
        var month = String.valueOf(now.getMonthValue());
        var day = String.valueOf(now.getDayOfMonth());
        var hour = String.valueOf(now.getHour());
        var minute = String.valueOf(now.getMinute());
        String second = "00";

        try {
            result = parseToTimestamp(year, month, day, hour, minute, second);
        } catch (Exception e) {
            // not possible
        }

        return result;
    }

    public static String parseDateToTimestamp(String year, String month, String day)
    throws IllegalArgumentException {
        String result = null;

        year = year.strip();
        month = month.strip();
        day = day.strip();

        result = parseToTimestamp(year, month, day, "00", "00" ,"00");

        return result;
    }

    public static String parseToTimestamp(String year, String month, String day,
    String hour, String minute, String second) throws IllegalArgumentException{
        String result = null;

        if (Integer.parseInt(hour) < 0 || Integer.parseInt(hour) > 60 ||
            Integer.parseInt(minute) < 0 || Integer.parseInt(minute) > 60 ||
            Integer.parseInt(second) < 0 || Integer.parseInt(second) > 60 ||
            Integer.parseInt(day) < 0 || Integer.parseInt(day) > 31 ||
            Integer.parseInt(month) < 0 || Integer.parseInt(month) > 12 ||
            Integer.parseInt(year) < LocalDate.now().getYear() - 200 ||
            Integer.parseInt(year) > LocalDate.now().getYear() + 200){
                throw new IllegalArgumentException("Date is of incorrect format.");
        }

        if (day.length() == 1) day = "0" + day;
        if (month.length() == 1) month = "0" + month;
        if (hour.length() == 1) hour = "0" + hour;
        if (minute.length() == 1) minute = "0" + minute;
        if (second.length() == 1) second = "0" + second;

        result = year + "-" + month + "-" + day + "T"
        + hour + ":" + minute + ":" + second + ".0Z";

        var temp_str = year + "-" + month + "-" + day + " "
        + hour + ":" + minute + ":" + second;

        Timestamp.valueOf(temp_str);
        // only for throwing errors

        return result;
    }
}
