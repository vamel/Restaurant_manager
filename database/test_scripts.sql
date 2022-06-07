--dish_production_cost
SELECT
    dish_production_cost(4)
FROM
    dual;   --1500=5*300 sam skladnik drugi
SELECT
    dish_production_cost(24)
FROM
    dual;  --Null - brak dodanych skladnikow


--monthly_balance
SELECT
    monthly_balance(1, 2022)
FROM
    dual;  --Styczen 2022, ok 143M
SELECT
    monthly_balance(12, 2021)
FROM
    dual; --Null - brak danych w bazie dla tego miesiaca


--add_reservation
EXEC add_reservation(3, TIMESTAMP '2022-01-20 19:00:00', 15, 'Niko Bellic');     --Rezerwacja do 19:15
EXEC add_reservation(3, TIMESTAMP '2022-01-20 23:00:00', 120, 'Niko Bellic');    --Rezerwacja do 1:00, powinnien zmienic sie tez dzien


--change_seats
SELECT
    seatcount
FROM
    tables
WHERE
    id = 3;    --Sprawdz ilosc siedzen przy stoliku 3
EXEC change_seats(3, 10);                   --Zmien ilosc siedzen na 10
SELECT
    seatcount
FROM
    tables
WHERE
    id = 3;    --Ilosc siedzen powinna wynosic 10


--print_tax
EXEC print_tax;


--tg_sum_seats
--przed testowaniem wlacz monitor dmbs output!!!
INSERT INTO tables VALUES (
    NULL,
    5,
    'trololo',
    1
);

DELETE FROM tables
WHERE
    id > 3;

UPDATE tables
SET
    seatcount = 9
WHERE
    seatcount = 5;


--tg_owner_bonus
INSERT INTO restaurants VALUES (
    NULL,
    'Testers diner',
    41,
    1,
    141
);     --User o id 141 powinien dostac 1000 podwyzki




--Testy bazy danych
--Dla ka�dego dania podaj ilo�� r�nych dostawc�w jego skadnik�w
select d.name, count(distinct supplierid) from
dishes d left join dishingredients j on (d.id = j.dishid)
left join ingredients i on (j.ingredientid = i.id)
left join supplieringredients r on (i.id = r.ingredientid)
left join suppliers s on (r.supplierid = s.id)
group by d.name;

--Dla ka�dego pracownika z niezerowa pensja policz stosunek lacznej wartosci jego zamowien do pensji
select u.id as employee_id, sum(totalprice)/avg(salary) as value
from customerorders c right join users u on (c.assignedemployeeid = u.id)
where salary > 0
group by u.id;

--Dla ka�dej restauracji policz laczna warto�� skladnikow jakie posiada
select r.name, sum(amount*price)
from restaurants r left join RESTAURANTINGREDIENTS j on (r.id = j.restaurantid)
join ingredients i on (j.ingredientid = i.id)
group by r.name;

--Dla ka�dego dostawcy policz laczna warto�� dostarczonych produktow
select s.name, sum(amount*price)
from suppliers s left join supplierorders j on (s.id = j.supplierid)
join orderingredients o on (j.id = o.supplierorderid)
join ingredients i on (o.ingredientid = i.id)
group by s.name;
