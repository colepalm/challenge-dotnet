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

            //Part 1:
            let toSend = JSON.parse(data._body);
            
            const dataReq = ctrl.http.post(ctrl.originUrl + '/api/SampleData', toSend, {headers: headers});
            
            dataReq.subscribe(function(response) {
                    console.log('Part 1 JSON Response:');
                    console.log(response);
                    console.log('\n\n');
                    ctrl.feesChallenge(response);
                });

            // //Part 2:
            // http.post('/api/distributions', data)
            //     .subscribe(function(response) {
            //     console.log('Part 2 JSON Response:');
            //     console.log(response);
            //     console.log('\n\n');
            //     this.distributionChallenge(response);
            // });
        });
    }

    private feesChallenge(response) {

    }

    private distributionChallenge(response) {

    }
}
