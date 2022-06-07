package projekt.pap;

import org.json.JSONObject;

public class EmployeeRecord {
    // simple class, purely for TableView purposes
    private String name;
    private String surname;
    private String birthDate;
    private int salary;
    private int id;

    public EmployeeRecord(int id, String name, String surname, String birthDate,
                          int salary, JSONObject data){
        this.id = id;
        this.surname = surname;
        this.name = name;
        this.birthDate = birthDate;
        this.salary = salary;
    }

    public String getId(){
        return String.valueOf(id);
    }

    public String getSalary(){
        return String.valueOf((double) salary / 100) + " zł";
    }

    public String getSurname(){
        return surname;
    }

    public String getName(){
        return name;
    }

    public String getBirthDate(){
        return birthDate.substring(0, 10);
    }
}
