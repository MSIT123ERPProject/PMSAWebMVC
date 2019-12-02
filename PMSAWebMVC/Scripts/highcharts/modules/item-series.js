/*
 Highcharts JS v7.2.1 (2019-10-31)

 Item series type for Highcharts

 (c) 2019 Torstein Honsi

 License: www.highcharts.com/license
*/
(function(a){"object"===typeof module&&module.exports?(a["default"]=a,module.exports=a):"function"===typeof define&&define.amd?define("highcharts/modules/item-series",["highcharts"],function(d){a(d);a.Highcharts=d;return a}):a("undefined"!==typeof Highcharts?Highcharts:void 0)})(function(a){function d(a,b,d,D){a.hasOwnProperty(b)||(a[b]=D.apply(null,d))}a=a?a._modules:{};d(a,"modules/item-series.src.js",[a["parts/Globals.js"],a["parts/Utilities.js"]],function(a,b){var d=b.defined,D=b.extend,y=b.isNumber,
E=b.objectEach,F=b.pick,G=a.fireEvent;b=a.merge;var l=a.seriesTypes.pie.prototype.pointClass.prototype;a.seriesType("item","pie",{endAngle:void 0,innerSize:"40%",itemPadding:.1,layout:"vertical",marker:b(a.defaultOptions.plotOptions.line.marker,{radius:null}),rows:void 0,showInLegend:!0,startAngle:void 0},{translate:function(){this.slots||(this.slots=[]);y(this.options.startAngle)&&y(this.options.endAngle)?(a.seriesTypes.pie.prototype.translate.call(this),this.slots=this.getSlots()):(this.generatePoints(),
G(this,"afterTranslate"))},getSlots:function(){function c(a){0<B&&(a.row.colCount--,B--)}for(var a=this.center,p=a[2],b=a[3],d,m=this.slots,r,x,t,u,v,C,h,w,f=0,n,z=this.endAngleRad-this.startAngleRad,q=Number.MAX_VALUE,A,k,g,e=this.options.rows,l=(p-b)/p;q>this.total;)for(A=q,q=m.length=0,k=g,g=[],f++,n=p/f/2,e?(b=(n-e)/n*p,0<=b?n=e:(b=0,l=1)):n=Math.floor(n*l),d=n;0<d;d--)t=(b+d/n*(p-b-f))/2,u=z*t,v=Math.ceil(u/f),g.push({rowRadius:t,rowLength:u,colCount:v}),q+=v+1;if(k){for(var B=A-this.total;0<
B;)k.map(function(a){return{angle:a.colCount/a.rowLength,row:a}}).sort(function(a,c){return c.angle-a.angle}).slice(0,Math.min(B,Math.ceil(k.length/2))).forEach(c);k.forEach(function(c){var b=c.rowRadius;C=(c=c.colCount)?z/c:0;for(w=0;w<=c;w+=1)h=this.startAngleRad+w*C,r=a[0]+Math.cos(h)*b,x=a[1]+Math.sin(h)*b,m.push({x:r,y:x,angle:h})},this);m.sort(function(a,c){return a.angle-c.angle});this.itemSize=f;return m}},getRows:function(){var a=this.options.rows;if(!a){var b=this.chart.plotWidth/this.chart.plotHeight;
a=Math.sqrt(this.total);if(1<b)for(a=Math.ceil(a);0<a;){var d=this.total/a;if(d/a>b)break;a--}else for(a=Math.floor(a);a<this.total;){d=this.total/a;if(d/a<b)break;a++}}return a},drawPoints:function(){var a=this,b=this.options,p=a.chart.renderer,l=b.marker,y=this.borderWidth%2?.5:1,m=0,r=this.getRows(),x=Math.ceil(this.total/r),t=this.chart.plotWidth/x,u=this.chart.plotHeight/r,v=this.itemSize||Math.min(t,u);this.points.forEach(function(c){var h,w,f=c.marker||{},n=f.symbol||l.symbol;f=F(f.radius,
l.radius);var z=d(f)?2*f:v,q=z*b.itemPadding,A;c.graphics=h=c.graphics||{};a.chart.styledMode||(w=a.pointAttribs(c,c.selected&&"select"));if(!c.isNull&&c.visible){c.graphic||(c.graphic=p.g("point").add(a.group));for(var k=0;k<c.y;k++){if(a.center&&a.slots){var g=a.slots.shift();var e=g.x-v/2;g=g.y-v/2}else"horizontal"===b.layout?(e=m%x*t,g=u*Math.floor(m/x)):(e=t*Math.floor(m/r),g=m%r*u);e+=q;g+=q;var C=A=Math.round(z-2*q);a.options.crisp&&(e=Math.round(e)-y,g=Math.round(g)+y);e={x:e,y:g,width:A,
height:C};void 0!==f&&(e.r=f);h[k]?h[k].animate(e):h[k]=p.symbol(n,null,null,null,null,{backgroundSize:"within"}).attr(D(e,w)).add(c.graphic);h[k].isActive=!0;m++}}E(h,function(a,b){a.isActive?a.isActive=!1:(a.destroy(),delete h[b])})})},drawDataLabels:function(){this.center&&this.slots?a.seriesTypes.pie.prototype.drawDataLabels.call(this):this.points.forEach(function(a){a.destroyElements({dataLabel:1})})},animate:function(a){a?this.group.attr({opacity:0}):(this.group.animate({opacity:1},this.options.animation),
this.animate=null)}},{connectorShapes:l.connectorShapes,getConnectorPath:l.getConnectorPath,setVisible:l.setVisible,getTranslate:l.getTranslate});""});d(a,"masters/modules/item-series.src.js",[],function(){})});
//# sourceMappingURL=item-series.js.map