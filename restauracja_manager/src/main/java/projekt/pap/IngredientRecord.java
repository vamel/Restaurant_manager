package projekt.pap;

public class IngredientRecord {
    // simple class, purely for TableView purposes
    private int id;
    private String name;
    private int amount;
    private boolean isPricePerKilogram;
    private int price;

    IngredientRecord(int id, int price, int amount, boolean isPricePerKG, String name){
        this.id = id;
        this.isPricePerKilogram = isPricePerKG;
        this.price = price;
        this.amount = amount;
        this.name = name;
    }

    public String getId(){
        return String.valueOf(id);
    }

    public String getPrice(){
        return String.valueOf((double) price / 100) + " zł";
    }

    public String getAmount(){
        return String.valueOf((double) amount / 1000) + " kg";
    }

    public String getName(){
        return name;
    }

    public String getIsPricePerKilogram(){
        return String.valueOf(isPricePerKilogram);
    }
}
