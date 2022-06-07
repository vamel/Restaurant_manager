package projekt.pap;

import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.http.HttpResponse;
import java.util.ArrayList;

import org.json.JSONArray;
import org.json.JSONObject;

import javafx.fxml.FXMLLoader;
import javafx.geometry.Insets;
import javafx.geometry.Pos;
import javafx.scene.Node;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.ChoiceBox;
import javafx.scene.control.Label;
import javafx.scene.control.ScrollPane;
import javafx.scene.control.SelectionMode;
import javafx.scene.control.TableView;
import javafx.scene.control.TextField;
import javafx.scene.control.cell.PropertyValueFactory;
import javafx.scene.layout.HBox;
import javafx.scene.layout.Priority;
import javafx.scene.layout.Region;
import javafx.scene.layout.VBox;
import javafx.stage.Stage;

public class SupplierIngredientOfferWindow {
    private VBox root;
    private Scene scene;
    private Stage stage;

    private Label infoStatus;
    
    private ArrayList<Integer> ingredientIds;
    private ChoiceBox<String> ingredientNames;

    private Integer supplierId;

    public SupplierIngredientOfferWindow(Integer supplierId) {
        this.supplierId = supplierId;
    }

    public void run() {
        stage = new Stage();
        stage.setTitle("Add Supplier Ingredient Offer");

        var loader = new FXMLLoader();

        try {
            loader.setLocation(new URL("file:src/main/resources/SupplierIngredientOffer.fxml"));
        } catch (MalformedURLException ex) {
            System.out.println(ex);
            return;
        }

        try {
            root = (VBox) loader.load();
        } catch (IOException ex) {
            System.out.println(ex);
            return;
        }

        scene = new Scene(root);
        stage.setScene(scene);
        stage.show();

        cacheControls();
        setupEvents();

        fillIngredientList();

    }

    private void cacheControls() {
        ingredientNames = (ChoiceBox<String>) scene.lookup("#IngredientList");

        infoStatus = (Label) scene.lookup("#Status");
    }

    private void setupEvents() {
        ingredientNames.valueProperty().addListener((ov, oldvalue, newvalue) -> {
            try {
                RestaurantAPI.addSupplierIngredient(supplierId, ingredientIds.get(ingredientNames.getSelectionModel().getSelectedIndex()), App.token);
                stage.close();
            } catch (IOException | InterruptedException ex) {
                infoStatus.setText(ex.getMessage());
            }
        });
    }

    private void fillIngredientList() {
        ingredientIds = new ArrayList<Integer>();
        
        HttpResponse<String> result;
        
        try {
            result = RestaurantAPI.getRestaurant(App.rest_id, App.token);
            
            if (result.statusCode() != 200) {
                var body = new JSONObject(result.body());
                infoStatus.setText(body.getString("Message"));
            }
            else {
                JSONArray ingredients = new JSONObject(result.body()).getJSONArray("restaurantIngredients");
            
            
                if (ingredients.length() == 0) {
                    infoStatus.setText("No ingredients in this restaurant.");
                    return;
                }

                for (int i = 0; i < ingredients.length(); i++) {
                    try {
                        var ingId = ingredients.getJSONObject(i).getInt("ingredientId");
                        ingredientIds.add(ingId);

                        var ingResult = RestaurantAPI.getIngredient(ingId, App.token);

                        if (ingResult.statusCode() != 200) {
                            var body = new JSONObject(ingResult.body());
                            infoStatus.setText(body.getString("Message"));
                        }
                        else {
                            JSONObject ingredient = new JSONObject(ingResult.body());

                            ingredientNames.getItems().add(ingredient.getString("name"));
                        }
                    } catch (IOException | InterruptedException ex) {
                        infoStatus.setText(ex.getMessage());
                    }
                }
            }
        }
        catch (IOException | InterruptedException ex) {
            infoStatus.setText(ex.getMessage());
        }
    }
}
