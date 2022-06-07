package projekt.pap;

public class SupplierRecord {
    private int id = 0;
    private String name = "Unnamed";

    public SupplierRecord(int id, String name){
        this.id = id;
        this.name = name;
    }

    public int getId(){
        return id;
    }

    public String getName(){
        return name;
    }
}
