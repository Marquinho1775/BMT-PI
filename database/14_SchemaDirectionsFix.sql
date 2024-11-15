delete from Order_Product
delete from Orders
delete from Directions

alter table Directions
drop constraint FK_Directions_UserName

alter table Directions
drop column UserName

alter table Directions
add UserId uniqueidentifier not null

alter table Directions
ADD CONSTRAINT FK_Directions_UserId foreign key (UserId) references Users(Id);