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

public class SupplierCreationWindow {
    private VBox root;
    private Scene scene;


    private Label infoStatus;
    
    private ArrayList<Integer> countryIds;
    private ChoiceBox<String> countryBox;

    private TextField nameField;
    private TextField bankField;
    private TextField addressCityField;
    private TextField addressPostalCodeField;
    private TextField addressStreetField;
    private TextField addressStreetNumberField;

    private Integer countryId;
    public void run() {
        var stage = new Stage();
        stage.setTitle("Supplier Creation");

        var loader = new FXMLLoader();

        try {
            loader.setLocation(new URL("file:src/main/resources/SupplierCreation.fxml"));
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

        fillCountryList();

    }

    private void cacheControls() {
        nameField = (TextField) scene.lookup("#NameField");
        nameField.setPromptText("eg. The Tomato Emporium");

        bankField = (TextField) scene.lookup("#BankField");
        bankField.setPromptText("eg. 0000 0000 0000 0000 0000 0000");

        addressCityField = (TextField) scene.lookup("#CityField");
        addressCityField.setPromptText("City");

        addressPostalCodeField = (TextField) scene.lookup("#PostalCodeField");
        addressPostalCodeField.setPromptText("ZIP");

        addressStreetField = (TextField) scene.lookup("#StreetNameField");
        addressStreetField.setPromptText("Street name");

        addressStreetNumberField = (TextField) scene.lookup("#StreetNumberField");
        addressStreetNumberField.setPromptText("no.");

        countryBox = (ChoiceBox<String>) scene.lookup("#CountryChoice");
        countryBox.setAccessibleText("Country");

        infoStatus = (Label) scene.lookup("#Status");
    }

    private void setupEvents() {
        countryBox.valueProperty().addListener((ov, oldvalue, newvalue) -> {
            countryId = countryIds.get(countryBox.getSelectionModel().getSelectedIndex());
        });

        Button createSupplierButton = (Button) scene.lookup("#CreateButton");

        createSupplierButton.setOnAction(ae -> {
            HttpResponse<String> result;
            
            try {
                result = RestaurantAPI.addSupplier(nameField.getText(), 
                    bankField.getText(), 
                    countryId, 
                    addressPostalCodeField.getText(), 
                    addressCityField.getText(), 
                    addressStreetField.getText(), 
                    addressStreetNumberField.getText(), 
                    App.token);

            } catch (IOException | InterruptedException ex) {
                infoStatus.setText(ex.getMessage());
                return;
            }

            if (result.statusCode() != 200)
            {
                infoStatus.setText((new JSONObject(result.body())).getString("Message"));
            }
            else {
                infoStatus.setText("Successfully created supplier.");
            }
        });
    }

    private void fillCountryList() {
        HttpResponse<String> result;
        
        try {
            result = RestaurantAPI.getCountries(App.token);
            
            if (result.statusCode() != 200) {
                var body = new JSONObject(result.body());
                infoStatus.setText(body.getString("Message"));
            }
            else {
                var countries = new JSONArray(result.body());
                countryIds = new ArrayList<Integer>();
                
                for (int i = 0; i < countries.length(); i++)
                {
                    var country = countries.getJSONObject(i);
                    countryBox.getItems().add(country.getString("name"));
                    countryIds.add(country.getInt("id"));
                }
            }
        }
        catch (IOException | InterruptedException ex) {
            infoStatus.setText(ex.getMessage());
        }
    }
}
