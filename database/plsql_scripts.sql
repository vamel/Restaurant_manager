--Funkcje
--Oblicz koszt produkcji dania o podanym id
CREATE OR REPLACE FUNCTION dish_production_cost (
    p_id NUMBER
) RETURN NUMBER AS
    v_cost_sum NUMBER;
BEGIN
    SELECT
        SUM(amount * i.price)
    INTO v_cost_sum
    FROM
             dishes d
        JOIN dishingredients j ON ( d.id = j.dishid )
        JOIN ingredients     i ON ( i.id = j.ingredientid )
    WHERE
        d.id = p_id;

    RETURN v_cost_sum;
END;
/
--Oblicz bilans dla danego miesiaca
CREATE OR REPLACE FUNCTION monthly_balance (
    p_month NUMBER,
    p_year  NUMBER
) RETURN NUMBER AS
    v_ing_spending NUMBER;
    v_emp_salary   NUMBER;
    v_income       NUMBER;
BEGIN

    --Oblicz wydatki na skladniki
    SELECT
        SUM(amount * price)
    INTO v_ing_spending
    FROM
             orderingredients o
        JOIN ingredients         i ON ( o.ingredientid = i.id )
        JOIN supplieringredients j ON ( i.id = j.ingredientid )
        JOIN supplierorders      d ON ( o.supplierorderid = d.id )
    WHERE
            EXTRACT(MONTH FROM "Date") = p_month
        AND EXTRACT(YEAR FROM "Date") = p_year;

    --Oblicz pensje pracownikow
    SELECT
        SUM(salary)
    INTO v_emp_salary
    FROM
        users;
    
    --Oblicz przychody z zamowien
    SELECT
        SUM(totalprice)
    INTO v_income
    FROM
        customerorders;

    RETURN v_income - v_emp_salary - v_ing_spending;
END;
/


--Procedury
--Dodaj rezerwacje
CREATE OR REPLACE PROCEDURE add_reservation (
    p_tableid   NUMBER,
    p_starttime TIMESTAMP,
    p_duration  NUMBER,
    p_name      NVARCHAR2
) AS
    v_endtime TIMESTAMP;
BEGIN
    v_endtime := p_starttime + ( 1 / 1440 ) * p_duration;
    INSERT INTO reservations VALUES (
        NULL,
        p_tableid,
        p_starttime,
        v_endtime,
        p_name
    );

END;
/
--Zmien ilosc siedzen przy stole
CREATE OR REPLACE PROCEDURE change_seats (
    p_id    NUMBER,
    p_seats NUMBER
) AS
BEGIN
    UPDATE tables
    SET
        seatcount = p_seats
    WHERE
        id = p_id;

END;
/
--Dla kazdego usera wypisz komunikat o wysokosci jego podatkow
CREATE OR REPLACE PROCEDURE print_tax AS
BEGIN
    FOR r_emp IN (
        SELECT
            *
        FROM
            users
    ) LOOP
        dbms_output.put_line('Tax for '
                             || r_emp.name
                             || ' '
                             || r_emp.surname
                             || ' is '
                             || r_emp.salary * 0.34);
    END LOOP;
END;
/



--Wyzwalacze
--Po dodaniu modyfikacji stolika wyswietl laczna ilosc siedzen 
CREATE OR REPLACE TRIGGER tg_sum_seats AFTER
    INSERT OR UPDATE OR DELETE ON tables
DECLARE
    v_sum_seats NUMBER;
BEGIN
    SELECT
        SUM(seatcount)
    INTO v_sum_seats
    FROM
        tables;

    dbms_output.put_line('Laczna ilosc siedzien to teraz ' || v_sum_seats);
END;
/
--Po dodaniu restauracji daj wlascicielowi 1000 podwyzki
CREATE OR REPLACE TRIGGER tg_owner_bonus AFTER
    INSERT ON restaurants
    FOR EACH ROW
DECLARE
    v_salary NUMBER;
BEGIN
    SELECT
        salary
    INTO v_salary
    FROM
        users
    WHERE
        id = :new.ownerid;

    UPDATE users
    SET
        salary = v_salary + 1000
    WHERE
        id = :new.ownerid;

END;
/