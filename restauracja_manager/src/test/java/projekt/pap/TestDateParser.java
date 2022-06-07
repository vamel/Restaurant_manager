package projekt.pap;

import static org.junit.jupiter.api.Assertions.fail;
import java.time.LocalDate;
import static org.junit.jupiter.api.Assertions.assertEquals;
import org.junit.jupiter.api.Test;

public class TestDateParser {
    @Test
    void testParseToTimestamp(){
        // parsing to String is done in UI classes

        //parseDateToTimestamp is parseToTimestamp with constant last 3 params,
        //so no need to test
        String result = null;

        try {
            //normal
            result = DateParser.parseToTimestamp("2022", "01", "13", "10","00","00");
            assertEquals(result.length(), 22);
            assertEquals(result.substring(0, 4), "2022");
            assertEquals(result.substring(5, 7), "01");
            assertEquals(result.substring(8, 10), "13");
        } catch (IllegalArgumentException e) {
            fail("Unable to parse normal date");
        }

        result = null;
        try {
            //single numbers
            result = DateParser.parseToTimestamp("2022", "1", "1", "1","0","0");
        } catch (IllegalArgumentException e) {
            fail("Unable to parse date with single numbers");
        }

        result = null;
        try {
            //wrong number values
            result = DateParser.parseToTimestamp("2022", "1", "91", "1","-30","0");
            fail("Parsed wrong date");
        } catch (IllegalArgumentException e) {}

        result = null;
        try {
            //text
            result = DateParser.parseToTimestamp("2022", "test", "31", "1","30","0");
            fail("Parsed date with text");
        } catch (IllegalArgumentException e) {}
    }

    @Test
    void testParseNow(){
        String result = DateParser.parseNow();
        var now = LocalDate.now().toString();
        assertEquals(result.length(), 22);
        assertEquals(result.substring(0, 4), now.substring(0, 4));
        assertEquals(result.substring(5, 7), now.substring(5, 7));
        assertEquals(result.substring(8, 10), now.substring(8, 10));
    }
}
