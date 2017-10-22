import { Component, Inject } from '@angular/core';
import { Http, Response } from '@angular/http';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent {
    
    private baseUrl: string;
    orders: Order[];
    orderLines: OrderLine[];
    products: Product[];
    currentOrder?: Order;
    orderId: number;
    searchMask: string;


    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string){
        this.baseUrl =  baseUrl;
        this.performSearch('');

        this.http.get(`${this.baseUrl}api/Order/Products`).subscribe(result => {
            this.products = result.json() as Product[];
            }, error => console.error(error));
    }

    performSearch(searchTerm: string){
        console.log(`User entered: ${searchTerm}`);
        this.http.get(`${this.baseUrl}api/Order/Orders?mask=${searchTerm}`).subscribe(result => {
            this.orders = result.json() as Order[];
            this.searchMask = searchTerm;
        }, error => console.error(error));
    }
    
    editOrder(orderId: number){
        this.http.get(`${this.baseUrl}api/Order/OrderLines?orderId=${orderId}`).subscribe(result => {
            this.orderLines = result.json() as OrderLine[];
            let index = this.orders.findIndex(order => order.id === orderId);
            this.currentOrder = this.orders[index];
            this.orderId = orderId;
        }, error => console.error(error));
    }

    deleteOrder(orderId: number){

        this.http.delete(`${this.baseUrl}api/Order/DeleteOrder`,
            {body: {OrderId: orderId, Mask: this.searchMask}}).subscribe(result => {
            this.orders = result.json() as Order[];
            if(this.orderId === orderId){
                if(this.orders.length > 0){
                    this.editOrder(0);
                }
                else {
                    this.currentOrder = undefined;
                    this.orderId = -1;
                    this.orderLines = [];
                }
            }
        }, error => console.error(error));

    }


    deleteOrderLine(orderLineId: number){
        let currentOrder = this.orderId;
        this.http.delete(`${this.baseUrl}api/Order/DeleteOrderLine`,
        {body: {OrderLineId: orderLineId, OrderId: this.orderId }}).subscribe(result => {
            if(this.orderId === currentOrder){
                this.orderLines = result.json() as OrderLine[];    
            }
        }, error => console.error(error));
    }

    addOrder(customerName: string){
        this.http.post(`${this.baseUrl}api/Order/AddOrder`, {CustomerName: customerName, Mask: this.searchMask}).subscribe(result => 
            {
                this.orders = result.json() as Order[];
            }, 
            error => console.error(error));
    }

    addOrderLine(productId: number, quantity: number) {

        this.http.post(`${this.baseUrl}api/Order/AddOrderLine`, 
        {OrderId: this.orderId, ProductId: productId, Quantity: quantity}).subscribe(result => 
            {
                this.orderLines = result.json() as OrderLine[];
                let totalPrice: number = 0;
                
                for(let orderLine of this.orderLines) {
                    for(let product of this.products) {
                        if(product.id === orderLine.productId){
                            totalPrice += orderLine.quantity * product.price;
                            break;
                        }
                    }
                }

                if(this.currentOrder === undefined){
                    return;
                }

                this.currentOrder.totalPrice = totalPrice;
            }, 
            error => console.error(error));
    }
    
}

//TODO: move to separate classes
interface Order {
    id: number;
    customerName: string;
    totalPrice: number;
}

class OrderLine {
    id: number;
    orderId: number;
    productId: number;
    productName: string;
    productPrice: number;
    quantity: number;
}

class Product {
    id: number;
    name: string;
    price: number;
}