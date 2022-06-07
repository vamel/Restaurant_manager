package projekt.pap;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotEquals;
import static org.junit.jupiter.api.Assertions.fail;
import java.io.IOException;
import java.net.http.HttpResponse;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.function.Executable;


public class TestRestaurantAPI {
    /* All test in this class require working backend in background
     * as it's pointless to test it without it working
    */

    @Test
    void testLogin(){
        HttpResponse<String> result = null;
        // invalid credentials
        try {
            result = RestaurantAPI.login("nie istnieje", "nie istnieje");
        } catch (InterruptedException | IOException e) {
            fail("Unable to connect to database");
        }
        assertNotEquals(result.statusCode(), 200, String.format("Logged in with invalid credentials, code: %d",
            result.statusCode()));

        result = null;
        try {
            result = RestaurantAPI.login("mario", "kart");
        } catch (InterruptedException | IOException e) {
            fail("Unable to connect to database");
        }

        assertEquals(result.statusCode(), 200, String.format("Request refused, code: %d", result.statusCode()));

        JSONObject result_json = null;
        try {
            result_json = new JSONObject(result.body());
        } catch (JSONException e) {
            fail("Result not in JSON format");
        }
        try {
            result_json.getString("token");
        } catch (JSONException e) {
            fail("Result not containing token field");
        }
        try {
            result_json.getInt("employeeId");
        } catch (JSONException e) {
            fail("Result not containing employeeId field");
        }
    }
    /* We need token for authorization for requests
    * only way to get it is to login
    * so we first test login, then use it before every test
    */
    @Nested
    class ProperTestRestaurantAPI{
        private String token = null;

        @BeforeEach
        void getToken(){
            HttpResponse<String> result = null;
            try {
                result = RestaurantAPI.login("mario", "kart");
            } catch (IOException | InterruptedException e) {
                fail("Unable to connect: getToken");
            }
            token = new JSONObject(result.body()).getString("token");
        }

        @Test
        void testGetEmployeeRole(){
            HttpResponse<String> result = null;
            try {
                result = RestaurantAPI.getEmployeeRole(1 ,token);
                assertEquals(result.statusCode(), 200,
                    String.format("Request refused, code: %d", result.statusCode()));
            } catch (IOException | InterruptedException e) {
                fail("Unable to connect");
            }

            String role = result.body();
            if(role.isBlank()) fail("Result doesn't contain role information");

            assertEquals(role.toLowerCase(), "owner", "Incorrect role as result");

            try {
                result = RestaurantAPI.getEmployeeRole(-1 ,token);
                assertNotEquals(result.statusCode(), 200,
                    String.format("Request should be refused, but isn't; code: %d", result.statusCode()));
            } catch (IOException | InterruptedException e) {
                fail("Unable to connect");
            }

            try {
                result = RestaurantAPI.getEmployeeRole(3 ,token);
                assertEquals(result.statusCode(), 200,
                    String.format("Request refused, code: %d", result.statusCode()));
            } catch (IOException | InterruptedException e) {
                fail("Unable to connect");
            }

            role =  result.body();
            if(role.isBlank()) fail("Result doesn't contain role information");
            assertEquals(role.toLowerCase(), "employee", "Incorrect role as result");
        }

        @Test
        void testRegisterEmployee(){
            // data type check is on frontend side and is done in NewEmployeeWindow
            HttpResponse<String> result = null;
            var name = "Test";
            try {
                // taken username
                result = RestaurantAPI.registerEmployee(name,"a","b","1234",1,null,
                DateParser.parseNow() ,token);
                assertNotEquals(result.statusCode(), 200,
                    String.format("Illegal request accepted, code: %d", result.statusCode()));
            } catch (IOException | InterruptedException e) {
                fail("Unable to connect");
            }
        }

        @Test
        void testGetFreeTables(){
            HttpResponse<String> result = null;
            try {
                // wrong restaurant
                result = RestaurantAPI.getFreeTables(2, DateParser.parseNow(),
                DateParser.parseNow(), token);
                assertNotEquals(result.statusCode(), 200,
                    String.format("Wrong request accepted, code: %d", result.statusCode()));
            } catch (IOException | InterruptedException e) {
               fail("Unable to connect");
            }
            result = null;
            try {
                // correct data
                result = RestaurantAPI.getFreeTables(1, DateParser.parseNow(),
                DateParser.parseNow(), token);
                assertEquals(result.statusCode(), 200,
                    String.format("Request refused, code: %d", result.statusCode()));
            } catch (IOException | InterruptedException e) {
                fail("Unable to connect");
            }

            JSONObject result_json = null;
            try {
                result_json = new JSONObject(result.body());
            } catch (JSONException e) {
                fail("Result not in JSON format");
            }

            JSONArray tables = null;
            try {
                tables = result_json.getJSONArray("tableIds");
            } catch (JSONException e) {
                fail("Result does not contain array of tables");
            }

            for (Object object : tables) {
                try {
                    int id = (int)object;
                } catch (Exception e) {
                    fail("Ids from result are not integers");
                }
            }
        }

        @Test
        void testGetEmployee(){
            HttpResponse<String> result = null;
            try {
                // wrong employee id
                result = RestaurantAPI.getEmployee(0, token);
                assertNotEquals(result.statusCode(), 200,
                    String.format("Wrong request accepted, code: %d", result.statusCode()));
            } catch (IOException | InterruptedException e) {
               fail("Unable to connect");
            }
            result = null;
            try {
                // invalid token
                result = RestaurantAPI.getEmployee(0, "123");
                assertNotEquals(result.statusCode(), 200,
                    String.format("Wrong request accepted, code: %d", result.statusCode()));
            } catch (IOException | InterruptedException e) {
                fail("Unable to connect");
            }
            result = null;
            try {
                // correct data
                result = RestaurantAPI.getEmployee(1, token);
                assertEquals(result.statusCode(), 200,
                    String.format("Request refused, code: %d", result.statusCode()));
            } catch (IOException | InterruptedException e) {
                fail("Unable to connect");
            }

            JSONObject result_json = null;
            try {
                result_json = new JSONObject(result.body());
            } catch (JSONException e) {
                fail("Result not in JSON format");
            }

            int rest_id = -2;
            try {
                rest_id = result_json.getInt("restaurantId");
                result_json.getString("name");
                result_json.getString("surname");
                result_json.getString("birthDate");
                result_json.getInt("salary");
            } catch (JSONException e) {
                fail("Result doesn't contain all required fields");
            }
            if(rest_id == -2) fail("Wrong id returned by API");
        }

        @Test
        void testAddDish(){
            Assertions.assertThrows(JSONException.class, new Executable() {
                @Override
                public void execute() throws Throwable{
                    // ingredient already there
                    new JSONObject(RestaurantAPI.addRestaurantIngredient(
                        1, 1, 100, token
                        ).body());
                }
            });

            Assertions.assertThrows(JSONException.class, new Executable() {
                @Override
                public void execute() throws Throwable{
                    // no ingredient like that
                    new JSONObject(RestaurantAPI.addRestaurantIngredient(
                        1, 100, 100, token
                        ).body());
                }
            });

            Assertions.assertThrows(JSONException.class, new Executable() {
                @Override
                public void execute() throws Throwable{
                    // negative amount
                    new JSONObject(RestaurantAPI.addRestaurantIngredient(
                        1, 2, -100, token
                        ).body());
                }
            });
        }

        @Test
        void testGetRestaurantEmployees(){
            Assertions.assertThrows(JSONException.class, new Executable() {
                @Override
                public void execute() throws Throwable{
                    // negative id, result not array
                    new JSONArray(RestaurantAPI.getRestaurantEmployees(
                        -2, token
                    ).body());
                }
            });

            try {
                new JSONArray(RestaurantAPI.getRestaurantEmployees(
                        1, token
                    ).body()); // correct usage
            } catch (IOException | InterruptedException e) {
                fail("unable to parse results to array");
            }

            Assertions.assertThrows(JSONException.class, new Executable() {
                @Override
                public void execute() throws Throwable{
                    // no rights for this restaurant
                    new JSONArray(RestaurantAPI.getRestaurantEmployees(
                        2, token
                    ).body());
                }
            });
        }
    }
}
