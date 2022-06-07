package projekt.pap;

import java.io.IOException;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.net.http.HttpResponse.BodyHandlers;
import java.util.ArrayList;

import org.json.JSONArray;
import org.json.JSONObject;


public class RestaurantAPI {
    private static final int NUMBER = 31166; // 50000 or 31166
    private static final String URL = String.format("http://localhost:%d/api/", NUMBER);

    public static HttpResponse<String> deleteDish (int id, String token)
    throws IOException, InterruptedException{
        var result = RestaurantAPI.executeDelete(URL + String.format("Menu/deleteDish/%d", id), token);
        return result;
    }

    public static HttpResponse<String> addDish (String name, int price, int menuId,
    JSONArray ingredients, String token) throws IOException, InterruptedException{

        var body = new JSONObject()
        .put("name", name)
        .put("price", price)
        .put("menuId", menuId)
        .put("dishIngredients", ingredients).toString();

        var result = RestaurantAPI.executePost(URL + "Menu/addDish", body, token);
        return result;
    }

    public static HttpResponse<String> addRestaurantIngredient (int rest_id, int ing_id, int amount, String token)
    throws IOException, InterruptedException{

        var body = new JSONObject()
        .put("restaurantId", rest_id).put("ingredientId", ing_id).put("amount", amount);

        var result = RestaurantAPI.executePost(URL + "Restaurant/addRestaurantIngredient", body.toString(), token);
        return result;
    }

    public static HttpResponse<String> addIngredient (String name, int price, boolean ippg, String token)
    throws IOException, InterruptedException{

        var body = new JSONObject()
        .put("isPricePerKilogram", ippg)
        .put("name", name)
        .put("price", price);

        var result = RestaurantAPI.executePost(URL + "Ingredient/addIngredient", body.toString(), token);
        return result;
    }

    public static HttpResponse<String> addSupplierOrder(int supplierId, ArrayList<Integer> ingredientIds,
            ArrayList<Float> amounts, String token)
            throws IOException, InterruptedException {
        JSONObject body = new JSONObject()
                .put("supplierId", supplierId)
                .put("restaurantId", App.rest_id);

        JSONArray ingredients = new JSONArray();
        for (int i = 0; i < ingredientIds.size(); i++) {
            if (amounts.get(i) != 0) {
                JSONObject ingredientOrder = new JSONObject()
                        .put("ingredientId", ingredientIds.get(i))
                        .put("amount", amounts.get(i));

                ingredients.put(ingredientOrder);
            }
        }

        body.put("orderIngredients", ingredients);

        var result = RestaurantAPI.executePost(URL + "Restaurant/addSupplierOrder", body.toString(), token);
        return result;
    }

    public static HttpResponse<String> addSupplierIngredient(Integer supplierId, Integer ingredientId, String token)
            throws IOException, InterruptedException {
        JSONObject body = new JSONObject()
                .put("supplierId", supplierId)
                .put("ingredientId", ingredientId);

        var result = RestaurantAPI.executePost(URL + "Supplier/addSupplierIngredient", body.toString(), token);
        return result;
    }

    public static HttpResponse<String> addSupplier(String name, String bankInfo, int countryId, String postalCode,
            String city, String street, String streetNumber, String token)
            throws IOException, InterruptedException {
        JSONObject body = new JSONObject()
                .put("name", name)
                .put("bankInformation", bankInfo);

        JSONObject address = new JSONObject()
                .put("countryId", countryId)
                .put("postalCode", postalCode)
                .put("city", city)
                .put("street", street)
                .put("streetNumber", streetNumber);

        body.put("addressId", (Object) null);
        body.put("address", address);

        var result = RestaurantAPI.executePost(URL + "Supplier/addSupplier", body.toString(), token);
        return result;
    }

    public static HttpResponse<String> getSuppliers(String token)
            throws IOException, InterruptedException {
        var result = RestaurantAPI.executeGet(URL + String.format("Supplier/getSuppliers"), token);
        return result;
    }

    public static HttpResponse<String> getCountries(String token)
            throws IOException, InterruptedException {
        var result = RestaurantAPI.executeGet(URL + String.format("Address/getAllCountries"), token);
        return result;
    }

    public static HttpResponse<String> fireEmployee (int id, String token)
    throws IOException, InterruptedException{
        var result = RestaurantAPI.executeDelete(URL + String.format("Employee/deleteEmployeeWorkingForYou/%d", id), token);
        return result;
    }

    public static HttpResponse<String> getIngredient (int ing_id, String token)
    throws IOException, InterruptedException{
        var result = RestaurantAPI.executeGet(URL + String.format("Ingredient/getIngredient/%d",
        ing_id), token);
        return result;
    }

    public static HttpResponse<String> updateEmployeeRole (String newRole, int employeeId, String token)
    throws IOException, InterruptedException{

        var body = new JSONObject()
        .put("employeeId", employeeId)
        .put("newRole", newRole);

        var result = RestaurantAPI.executePost(URL + "Authentication/updateEmployeeRole", body.toString(), token);
        return result;
    }

    public static HttpResponse<String> updateEmployeeSalary (int id, int salary, String token)
    throws IOException, InterruptedException{

        var body = new JSONObject()
        .put("id", id)
        .put("salary", salary);

        var result = RestaurantAPI.executePost(URL + "Employee/updateEmployeeWorkingForYou", body.toString(), token);
        return result;
    }

    public static HttpResponse<String> getRestaurantEmployees (int rest_id, String token)
    throws IOException, InterruptedException{
        var result = RestaurantAPI.executeGet(URL +
            String.format("Employee/getRestaurantEmployees/%d", rest_id), token);
        return result;
    }

    public static HttpResponse<String> getOwnedRestaurants (int owner_id, String token)
    throws IOException, InterruptedException{
        var result = RestaurantAPI.executeGet(URL + String.format("Restaurant/getOwnedRestaurants/%d",
        owner_id), token);
        return result;
    }

    public static HttpResponse<String> getRestaurant (int res_id, String token)
    throws IOException, InterruptedException{
        var result = RestaurantAPI.executeGet(URL + String.format("Restaurant/getRestaurant/%d",
        res_id), token);
        return result;
    }

    public static HttpResponse<String> getMenuWithDishes (int res_id, int menu_id, String token)
    throws IOException, InterruptedException{
        var result = RestaurantAPI.executeGet(URL + String.format("Menu/getMenuWithDishAvailability/%d/%d",
        res_id, menu_id), token);
        return result;
    }

    public static HttpResponse<String> addOrder (boolean takeout, String time, int price, int emp_id,
    int res_id, JSONArray dishes, String token) throws IOException, InterruptedException{

        var body = new JSONObject()
        .put("takeout", takeout)
        .put("orderTime", time)
        .put("totalPrice", price)
        .put("assignedEmployeeId", emp_id)
        .put("restaurantId", res_id)
        .put("dishes", dishes).toString();

        var result = RestaurantAPI.executePost(URL + "CustomerOrder/addOrder", body, token);
        return result;
    }

    public static HttpResponse<String> addReservation(int tableId, String sTime, String eTime,
    String name, String token) throws IOException, InterruptedException{
        String body = new JSONObject()
        .put("tableId", tableId)
        .put("startTime", sTime)
        .put("endTime", eTime)
        .put("name", name).toString();

        var result = RestaurantAPI.executePost(URL + "Reservation/addReservation", body , token);
        return result;
    }

    public static HttpResponse<String> getEmployee(int id, String token)
    throws IOException, InterruptedException{
        var result = RestaurantAPI.executeGet(URL + String.format("Employee/getEmployee/%d", id), token);
        return result;
    }

    public static HttpResponse<String> getFreeTables(int restaurantId, String startTime,
    String endTime, String token) throws IOException, InterruptedException{
        var body = new JSONObject()
        .put("restaurantId", restaurantId)
        .put("startTime", startTime)
        .put("endTime", endTime);

        var result = RestaurantAPI.executePost(URL + "Reservation/getFreeTablesDuring", body.toString(), token);

        return result;
    }

    public static HttpResponse<String> registerEmployee(String username, String name, String surname, String password,
    int restaurantID, String pesel, String birthdate, String token) throws IOException, InterruptedException {

        var body = new JSONObject()
        .put("username", username)
        .put("name", name)
        .put("surname", surname)
        .put("password", password)
        .put("restaurantId", restaurantID)
        .put("pesel", pesel)
        .put("birthDate", birthdate);

        var result = RestaurantAPI.executePost(URL + "Authentication/registerEmployee", body.toString(), token);

        return result;
    }

    public static HttpResponse<String> getEmployeeRole(int id, String token) throws IOException, InterruptedException{
        var result = RestaurantAPI.executeGet(URL + String.format("Employee/getEmployeeRole/%d", id), token);
        return result;
    }

    public static HttpResponse<String> login(String login, String password) throws IOException, InterruptedException{

        var body = new JSONObject()
        .put("username", login)
        .put("password", password);

        var result = RestaurantAPI.executePost(URL + "authentication/login", body.toString(), null);

        return result;
    }

    private static HttpResponse<String> executeDelete(String targetURL, String token) throws IOException, InterruptedException{
        HttpRequest request = null;
        var client = HttpClient.newHttpClient();

        request = HttpRequest.newBuilder(URI.create(targetURL))
            .header("Authorization", "Bearer " + token)
            .DELETE()
            .build();

        HttpResponse<String> response = client.send(request, BodyHandlers.ofString());
        return response;
    }

    private static HttpResponse<String> executeGet(String targetURL, String token) throws IOException, InterruptedException{
        HttpRequest request = null;
        var client = HttpClient.newHttpClient();

        request = HttpRequest.newBuilder(URI.create(targetURL))
            .header("Authorization", "Bearer " + token)
            .GET()
            .build();

        HttpResponse<String> response = client.send(request, BodyHandlers.ofString());
        return response;
    }

    private static HttpResponse<String> executePost(String targetURL, String body, String token) throws IOException, InterruptedException {
        var client = HttpClient.newHttpClient();
        HttpRequest request = null;

        if (!(token == null)){
            request = HttpRequest.newBuilder(URI.create(targetURL))
            .header("Content-Type", "application/json")
            .header("Authorization", "Bearer " + token)
            .POST(HttpRequest.BodyPublishers.ofString(body))
            .build();
        }
        else{
            request = HttpRequest.newBuilder(URI.create(targetURL))
            .header("Content-Type", "application/json")
            .POST(HttpRequest.BodyPublishers.ofString(body))
            .build();
        }

        HttpResponse<String> response = client.send(request, BodyHandlers.ofString());
        return response;
  }
}
