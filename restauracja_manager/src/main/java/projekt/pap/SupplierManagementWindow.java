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

public class SupplierManagementWindow {
    private HBox root;
    private Scene scene;

    private Label infoName;
    private Label infoBank;
    private Label infoCityCountry;
    private Label infoPostalCodeStreet;

    private Label infoStatus;
    
    private TableView<SupplierRecord> supplierList;
    private ScrollPane ingredientList;

    private JSONArray suppliers;
    private JSONObject currentSupplier;

    private ArrayList<TextField> orderFields;
    private ArrayList<Integer> ingredientIds;

    public void run() {
        var stage = new Stage();
        stage.setTitle("Supplier Management");

        var loader = new FXMLLoader();

        try {
            loader.setLocation(new URL("file:src/main/resources/SupplierManagement.fxml"));
        } catch (MalformedURLException ex) {
            System.out.println(ex);
            return;
        }

        try {
            root = (HBox) loader.load();
        } catch (IOException ex) {
            System.out.println(ex);
            return;
        }

        scene = new Scene(root);
        stage.setScene(scene);
        stage.show();

        cacheControls();
        setupEvents();

        grabSupplierInfo();

        fillSupplierList();
        fillWithData(1);
    }

    private void cacheControls() {
        infoName = (Label) scene.lookup("#InfoName");
        infoBank = (Label) scene.lookup("#InfoBank");
        infoCityCountry = (Label) scene.lookup("#InfoCityCountry");
        infoPostalCodeStreet = (Label) scene.lookup("#InfoPostalCodeStreet");

        ingredientList = (ScrollPane) scene.lookup("#IngredientList");
        supplierList = (TableView<SupplierRecord>) scene.lookup("#SupplierList");
        supplierList.getSelectionModel().setSelectionMode(SelectionMode.SINGLE);

        supplierList.getColumns().get(0).setCellValueFactory(new PropertyValueFactory<>("id"));
        supplierList.getColumns().get(1).setCellValueFactory(new PropertyValueFactory<>("name"));

        infoStatus = (Label) scene.lookup("#InfoStatus");
    }

    private void setupEvents() {
        supplierList.getSelectionModel().selectedItemProperty().addListener((obs, oldSelect, newSelect) -> {
            if (newSelect != null) {
                fillWithData(newSelect.getId());
            }
        });

        Button addSupplierButton = (Button) scene.lookup("#AddNewSupplier");
        Button addNewIngredient = (Button) scene.lookup("#AddNewIngredient");
        Button placeOrder = (Button) scene.lookup("#PlaceOrder");

        placeOrder.setOnAction(ae -> {
            ArrayList<Float> amounts = new ArrayList<Float>();

            for (TextField tf : orderFields) {
                amounts.add(Float.parseFloat(tf.getText()));
            }

            HttpResponse<String> result;
            try {
                result = RestaurantAPI.addSupplierOrder(currentSupplier.getInt("id"), ingredientIds, amounts, App.token);
            } catch (IOException | InterruptedException ex) {
                infoStatus.setText(ex.getMessage());
                return;
            }

            if (result.statusCode() != 200)
            {
                infoStatus.setText((new JSONObject(result.body())).getString("Message"));
            }
            else {
                infoStatus.setText("Successfully placed order.");
            }
        });

        addSupplierButton.setOnAction(ae -> {
            var window = new SupplierCreationWindow();
            window.run();
        });

        addNewIngredient.setOnAction(ae -> {
            var window = new SupplierIngredientOfferWindow(currentSupplier.getInt("id"));
            window.run();
        });
    }

    private void grabSupplierInfo() {
        HttpResponse<String> result;

        try {
            result = RestaurantAPI.getSuppliers(App.token);
        } catch (IOException | InterruptedException e) {
            System.out.println("Connection error");
            return;
        }

        suppliers = new JSONArray(result.body());
    }

    private void fillSupplierList() {
        for (Object o : suppliers) {
            var supplier = (JSONObject) o;

            supplierList.getItems().add(new SupplierRecord(supplier.getInt("id"), supplier.getString("name")));
        }
    }

    private void fillWithData(int id) {
        for (Object o : suppliers) {
            int cid = ((JSONObject)o).getInt("id");
            if (cid == id) {
                currentSupplier = (JSONObject) o;
                break;
            }
        }

        infoName.setText(String.format("%s (ID: %d)", currentSupplier.getString("name"), currentSupplier.getInt("id")));
        infoBank.setText(String.format("Bank information: %s", currentSupplier.getString("bankInformation")));

        var address = currentSupplier.getJSONObject("address");
        infoCityCountry.setText(String.format("%s, %s", address.getString("city"), address.getJSONObject("country").getString("name")));
        infoPostalCodeStreet.setText(String.format("%s, %s, %s", address.getString("postalCode"), address.getString("street"), address.getString("streetNumber")));

        var vbox = new VBox();
        var ingredients = currentSupplier.getJSONArray("ingredients");

        vbox.setMaxHeight(32 * ingredients.length());

        orderFields = new ArrayList<TextField>();
        ingredientIds = new ArrayList<Integer>();

        for (Object o : ingredients) {
            try {
                var result = RestaurantAPI.getIngredient(((JSONObject)o).getInt("ingredientId"), App.token);

                var line = generateIngredientRow(new JSONObject(result.body()));
                vbox.getChildren().add(line);
            }
            catch (IOException | InterruptedException e) {
                System.out.println(e);
            }
        }

        ingredientList.setContent(vbox);
    }

    private Node generateIngredientRow(JSONObject ingredient) {
        var ingredientRow = new HBox();
        ingredientRow.setAlignment(Pos.CENTER);
        ingredientRow.maxHeight(32);
        ingredientRow.minHeight(32);

        var nameLabel = new Label(ingredient.getString("name"));

        ingredientRow.getChildren().add(nameLabel);

        var spacer = new Region();
        spacer.setPrefSize(30, 32);
        HBox.setHgrow(spacer, Priority.ALWAYS);
        ingredientRow.getChildren().add(spacer);

        var priceLabel = new Label("Price: ");
		priceLabel.setPadding(new Insets(2, 4, 4, 2));

        ingredientRow.getChildren().add(priceLabel);

        var priceAmountLabel = new Label(String.format("%f zł", ingredient.getInt("price") * 0.01));
		priceAmountLabel.setPadding(new Insets(2, 4, 4, 2));

        ingredientRow.getChildren().add(priceAmountLabel);

        var orderAmount = new Label("Order amount: ");
		orderAmount.setPadding(new Insets(2, 4, 4, 2));

        ingredientRow.getChildren().add(orderAmount);

        var orderCount = new TextField();
        orderCount.setPromptText("count");
		orderCount.setMaxSize(64, 26);
		orderCount.setMinSize(64, 26);
        orderCount.setTextFormatter(new DecimalTextFormatter(0, 2));

        orderFields.add(orderCount);
        ingredientIds.add(ingredient.getInt("id"));


        ingredientRow.getChildren().add(orderCount);
        

        return ingredientRow;
    }
}
