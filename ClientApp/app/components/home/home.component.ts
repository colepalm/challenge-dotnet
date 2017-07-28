import {Component, InjectionToken, Inject} from '@angular/core';
import { Http, Headers } from '@angular/http';

export const ORIGIN_URL = new InjectionToken<string>('ORIGIN_URL');

export interface IHomeComponent {
    http: Http, 
    originUrl: string
}

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})

export class HomeComponent implements IHomeComponent {
    
    http: Http;
    originUrl: string;

    constructor(http: Http, @Inject('ORIGIN_URL')originUrl: string) { 
        this.originUrl = originUrl;
        this.http = http;    
    }

    ngOnInit(): void {
        let ctrl = this;

        ctrl.http.get('https://api.myjson.com/bins/u990d').subscribe(function (data: any) {
            //For API Challenge
            
            let headers = new Headers();
            headers.append('Content-Type', 'application/json');

            let toSend = JSON.parse(data._body);

            //Part 1:            
            const dataReq = ctrl.http.post(ctrl.originUrl + '/api/Price', toSend, {headers: headers});
            
            dataReq.subscribe(function(response: any) {
                    console.log('Part 1 JSON Response:');
                    let responseJson = JSON.parse(response._body);
                    
                    console.log(responseJson);
                    
                    console.log('\n\n');
                    ctrl.feesChallenge(responseJson);
                });

            //Part 2:
            const distReq = ctrl.http.post(ctrl.originUrl + '/api/Dist', toSend, {headers: headers});            
            
            distReq.subscribe(function(response: any) {
                console.log('Part 2 JSON Response:');
                
                let responseJson = JSON.parse(response._body);
                
                console.log(responseJson);
                console.log('\n\n');
                ctrl.distributionChallenge(responseJson);
            });
        });
    }

    private feesChallenge(orderData) {
        console.log('Part 1: Fees \n\n');

        orderData.forEach(function(order) {

            console.log('Order ID: ' + order.id);

            order.orderItems.forEach(function(orderItem) {
                console.log('\tOrder item ' + orderItem.type + ': $' + orderItem.amount);
            });


            console.log('\tOrder total: $' + order.total + '\n\n\n');
        });
    }

    private distributionChallenge(orderData) {
        console.log('Part 2: Distributions\n\n');

        orderData.forEach(function (order) {
            if (order.date !== null) {
                console.log('Order ID: ' + order.orderNumber);

                order.distributions.forEach(function (dist) {
                    console.log('\tFund - ' + dist.name + ': $' + dist.amount);
                })
            }
        });
        
        console.log('Total distributions:');
        
        orderData.forEach(function (order) {
            if (order.totals !== null) {
                
                for (let key in order.totals) {
                    console.log('\tFund - ' + key + ': $' + order.totals[key]);
                }
            }
        })
    }
}
