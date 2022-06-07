package projekt.pap;

public class DishRecord{
    // simple class, purely for TableView purposes
    private int id = 0;
    private int price = 0;
    private String name = "Unnamed";

    public DishRecord(int id, String name, int price){
        this.id = id;
        this.price = price;
        this.name = name;
    }

    public String getId(){
        return String.valueOf(id);
    }

    public String getPrice(){
        return String.valueOf((double) price / 100) + " zł";
    }

    public int getPriceReal(){
        return price;
    }

    public String getName(){
        return name;
    }
}
